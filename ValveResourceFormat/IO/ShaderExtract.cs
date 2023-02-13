using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;
using ValveResourceFormat.Utils;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace ValveResourceFormat.IO;

public sealed class ShaderExtract
{
    public readonly struct ShaderExtractParams
    {
        public bool CollapseCommonBuffers_InInclude { get; init; }
        public bool CollapseCommonBuffers_InPlace { get; init; }
        public static HashSet<string> CommonBuffers => new()
        {
            "PerViewConstantBuffer_t",
            "PerViewConstantBufferVR_t",
            "PerViewLightingConstantBufferVr_t",
            "DotaGlobalParams_t",
        };

        public bool FirstVsInput_Only { get; init; }

        public static readonly ShaderExtractParams Inspect = new()
        {
            CollapseCommonBuffers_InPlace = true,
            FirstVsInput_Only = true,
        };

        public static readonly ShaderExtractParams Export = new()
        {
            CollapseCommonBuffers_InInclude = true,
            FirstVsInput_Only = true,
        };
    }

    public ShaderCollection Shaders { get; init; }

    public ShaderFile Features => Shaders.Features;
    public ShaderFile Vertex => Shaders.Vertex;
    public ShaderFile Geometry => Shaders.Geometry;
    public ShaderFile Domain => Shaders.Domain;
    public ShaderFile Hull => Shaders.Hull;
    public ShaderFile Pixel => Shaders.Pixel;
    public ShaderFile Compute => Shaders.Compute;
    public ShaderFile PixelShaderRenderState => Shaders.PixelShaderRenderState;
    public ShaderFile Raytracing => Shaders.Raytracing;

    private ShaderExtractParams Options { get; set; }
    private List<string> FeatureNames { get; set; }
    private string[] Globals { get; set; }

    public ShaderExtract(Resource resource)
        : this((SboxShader)resource.DataBlock)
    { }

    public ShaderExtract(SboxShader sboxShaderCollection)
        : this(sboxShaderCollection.Shaders)
    { }

    public ShaderExtract(ShaderCollection shaderCollection)
    {
        Shaders = shaderCollection;
        ThrowIfNoFeatures();
    }

    private void ThrowIfNoFeatures()
    {
        if (Features == null)
        {
            throw new InvalidOperationException("Shader extract cannot continue without at least a features file.");
        }
    }

    public ContentFile ToContentFile()
    {
        var vfx = new ContentFile
        {
            Data = Encoding.UTF8.GetBytes(ToVFX(ShaderExtractParams.Export))
        };

        // TODO: includes..

        return vfx;
    }

    public string ToVFX()
    {
        return ToVFX(ShaderExtractParams.Inspect);
    }

    public string ToVFX(ShaderExtractParams options)
    {
        // TODO: IndentedTextWriter
        FeatureNames = Features.SfBlocks.Select(f => f.Name).ToList();
        Globals = Features.ParamBlocks.Select(p => p.Name).ToArray();
        Options = options;

        return "//=================================================================================================\n"
            + "// Reconstructed with VRF - https://vrf.steamdb.info/\n"
            + "//=================================================================================================\n"
            + HEADER()
            + MODES()
            + FEATURES()
            + COMMON()
            + VS()
            + GS()
            + HS()
            + DS()
            + PS()
            + CS()
            + RTX()
            ;
    }

    private string HEADER()
    {
        using var writer = new IndentedTextWriter();
        writer.WriteLine(nameof(HEADER));
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine($"Description = \"{Features.FeaturesHeader.FileDescription}\";");
        writer.WriteLine($"DevShader = {(Features.FeaturesHeader.DevShader == 0 ? "false" : "true")};");
        writer.WriteLine($"Version = {Features.FeaturesHeader.Version};");

        writer.Indent--;
        writer.WriteLine("}");

        return writer.ToString();
    }

    private string MODES()
    {
        using var writer = new IndentedTextWriter();
        writer.WriteLine(nameof(MODES));
        writer.WriteLine("{");
        writer.Indent++;

        foreach (var mode in Features.FeaturesHeader.Modes)
        {
            if (string.IsNullOrEmpty(mode.Shader))
            {
                writer.WriteLine($"{mode.Name}({mode.StaticConfig});");
            }
            else
            {
                writer.WriteLine($"{mode.Name}(\"{mode.Shader}\");");
            }
        }

        writer.Indent--;
        writer.WriteLine("}");
        return writer.ToString();
    }

    private string FEATURES()
    {
        using var writer = new IndentedTextWriter();
        writer.WriteLine();
        writer.WriteLine(nameof(FEATURES));
        writer.WriteLine("{");
        writer.Indent++;

        HandleFeatures(Features.SfBlocks, Features.SfConstraintsBlocks, writer);

        writer.Indent--;
        writer.WriteLine("}");

        return writer.ToString();
    }

    private string COMMON()
    {
        using var writer = new IndentedTextWriter();
        writer.WriteLine();
        writer.WriteLine(nameof(COMMON));
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("#include \"system.fxc\"");

        if (Vertex is not null)
        {
            HandleCBuffers(Vertex.BufferBlocks, writer);
        }

        HandleVsInput(writer);

        writer.Indent--;
        writer.WriteLine("}");

        return writer.ToString();
    }

    private void HandleVsInput(IndentedTextWriter writer)
    {
        if (Vertex is null)
        {
            return;
        }

        foreach (var i in Enumerable.Range(0, Vertex.SymbolBlocks.Count))
        {
            writer.WriteLine();

            var index = Vertex.SymbolBlocks.Count > 1 && !Options.FirstVsInput_Only
                ? $" ({i})"
                : string.Empty;
            writer.WriteLine($"struct VS_INPUT{index}");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (var symbol in Vertex.SymbolBlocks[i].SymbolsDefinition)
            {
                var attributeVfx = symbol.Option.Length > 0 ? $" < Semantic({symbol.Option}); >" : string.Empty;
                // TODO: type
                writer.WriteLine($"float4 {symbol.Name} : {symbol.Type}{symbol.SemanticIndex}{attributeVfx};");
            }

            writer.Indent--;
            writer.WriteLine("};");

            if (Options.FirstVsInput_Only)
            {
                break;
            }
        }
    }

    private void HandleCBuffers(List<BufferBlock> bufferBlocks, IndentedTextWriter writer)
    {
        var includedCommon = false;

        foreach (var buffer in bufferBlocks)
        {
            if (ShaderExtractParams.CommonBuffers.Contains(buffer.Name))
            {
                if (includedCommon)
                {
                    continue;
                }

                if (Options.CollapseCommonBuffers_InInclude)
                {
                    writer.WriteLine("#include \"common.fxc\"");
                    includedCommon = true;
                    continue;
                }


                if (Options.CollapseCommonBuffers_InPlace)
                {
                    writer.WriteLine("cbuffer " + buffer.Name + ";");
                    continue;
                }
            }

            writer.WriteLine();
            writer.WriteLine("cbuffer " + buffer.Name);
            writer.WriteLine("{");
            writer.Indent++;

            foreach (var member in buffer.BufferParams)
            {
                var dim1 = member.VectorSize > 1
                    ? member.VectorSize.ToString(CultureInfo.InvariantCulture)
                    : string.Empty;

                var dim2 = member.Depth > 1
                    ? "x" + member.Depth.ToString(CultureInfo.InvariantCulture)
                    : string.Empty;

                var array = member.Length > 1
                    ? "[" + member.Length.ToString(CultureInfo.InvariantCulture) + "]"
                    : string.Empty;

                writer.WriteLine($"float{dim1}{dim2} {member.Name}{array};");
            }

            writer.Indent--;
            writer.WriteLine("};");
        }
    }

    private string VS()
        => HandleStageCommons(Vertex, nameof(VS));

    private string GS()
        => HandleStageCommons(Geometry, nameof(GS));

    private string HS()
        => HandleStageCommons(Hull, nameof(HS));

    private string DS()
        => HandleStageCommons(Domain, nameof(DS));

    private string PS()
        => HandleStageCommons(Pixel, nameof(PS));

    private string CS()
        => HandleStageCommons(Compute, nameof(CS));

    private string RTX()
        => HandleStageCommons(Raytracing, nameof(RTX));

    private string HandleStageCommons(ShaderFile shader, string stageName)
    {
        if (shader is null)
        {
            return string.Empty;
        }

        using var writer = new IndentedTextWriter();
        writer.WriteLine();
        writer.WriteLine(stageName);
        writer.WriteLine("{");
        writer.Indent++;

        HandleStaticCombos(shader.SfBlocks, shader.SfConstraintsBlocks, writer);
        HandleDynamicCombos(shader.SfBlocks, shader.DBlocks, shader.DConstraintsBlocks, writer);
        HandleParameters(shader.ParamBlocks, writer);

        writer.Indent--;
        writer.WriteLine("}");
        return writer.ToString();
    }

    private void HandleFeatures(List<SfBlock> features, List<SfConstraintsBlock> constraints, IndentedTextWriter writer)
    {
        foreach (var feature in features)
        {
            var checkboxNames = feature.CheckboxNames.Count > 0
                ? " (" + string.Join(", ", feature.CheckboxNames.Select((x, i) => $"{i}=\"{x}\"")) + ")"
                : string.Empty;

            // Verify RangeMin
            writer.WriteLine($"Feature( {feature.Name}, {feature.RangeMin}..{feature.RangeMax}{checkboxNames}, \"{feature.Category}\" );");
        }

        HandleRules(ConditionalType.Feature,
                    Enumerable.Empty<ICombo>().ToList(),
                    Enumerable.Empty<ICombo>().ToList(),
                    constraints.Cast<IComboConstraints>().ToList(),
                    writer);
    }

    private void HandleStaticCombos(List<SfBlock> staticCombos, List<SfConstraintsBlock> constraints, IndentedTextWriter writer)
    {
        HandleCombos(ConditionalType.Static, staticCombos.Cast<ICombo>().ToList(), writer);
        HandleRules(ConditionalType.Static,
                    staticCombos.Cast<ICombo>().ToList(),
                    Enumerable.Empty<ICombo>().ToList(),
                    constraints.Cast<IComboConstraints>().ToList(),
                    writer);
    }

    private void HandleDynamicCombos(List<SfBlock> staticCombos, List<DBlock> dynamicCombos, List<DConstraintsBlock> constraints, IndentedTextWriter writer)
    {
        HandleCombos(ConditionalType.Dynamic, dynamicCombos.Cast<ICombo>().ToList(), writer);
        HandleRules(ConditionalType.Dynamic,
                    staticCombos.Cast<ICombo>().ToList(),
                    dynamicCombos.Cast<ICombo>().ToList(),
                    constraints.Cast<IComboConstraints>().ToList(),
                    writer);
    }

    private void HandleCombos(ConditionalType comboType, List<ICombo> combos, IndentedTextWriter writer)
    {
        foreach (var staticCombo in combos)
        {
            if (staticCombo.FeatureIndex != -1)
            {
                writer.WriteLine($"{comboType}Combo( {staticCombo.Name}, {FeatureNames[staticCombo.FeatureIndex]}, Sys( ALL ) );");
                continue;
            }
            else if (staticCombo.RangeMax != 0)
            {
                writer.WriteLine($"{comboType}Combo( {staticCombo.Name}, {staticCombo.RangeMin}..{staticCombo.RangeMax}, Sys( ALL ) );");
            }
            else
            {
                writer.WriteLine($"{comboType}Combo( {staticCombo.Name}, {staticCombo.RangeMax}, Sys( {staticCombo.Sys} ) );");
            }
        }
    }

    private void HandleRules(ConditionalType conditionalType, List<ICombo> staticCombos, List<ICombo> dynamicCombos, List<IComboConstraints> constraints, IndentedTextWriter writer)
    {
        foreach (var rule in HandleConstraintsY(staticCombos, dynamicCombos, constraints))
        {
            if (conditionalType == ConditionalType.Feature)
            {
                writer.WriteLine($"FeatureRule( {rule.Constraint}, \"{rule.Description}\" );");
            }
            else
            {
                writer.WriteLine($"{conditionalType}ComboRule( {rule.Constraint} );");
            }
        }
    }

    private IEnumerable<(string Constraint, string Description)> HandleConstraintsY(List<ICombo> staticCombos, List<ICombo> dynamicCombos, List<IComboConstraints> constraints)
    {
        foreach (var constraint in constraints)
        {
            var constrainedNames = new List<string>(constraint.ConditionalTypes.Length);
            foreach ((var Type, var Index) in Enumerable.Zip(constraint.ConditionalTypes, constraint.Indices))
            {
                if (Type == ConditionalType.Feature)
                {
                    constrainedNames.Add(FeatureNames[Index]);
                }
                else if (Type == ConditionalType.Static)
                {
                    constrainedNames.Add(staticCombos[Index].Name);
                }
                else if (Type == ConditionalType.Dynamic)
                {
                    constrainedNames.Add(dynamicCombos[Index].Name);
                }
            }

            // By value constraint
            // e.g. FeatureRule( Requires1( F_REFRACT, F_TEXTURE_LAYERS=0, F_TEXTURE_LAYERS=1 ), "Refract requires Less than 2 Layers due to DX9" );
            if (constraint.Values.Length > 0)
            {
                if (constraint.Values.Length != constraint.ConditionalTypes.Length - 1)
                {
                    throw new InvalidOperationException("Expected to have 1 less value than conditionals.");
                }

                constrainedNames = constrainedNames.Take(1).Concat(constrainedNames.Skip(1).Select((s, i) => $"{s} == {constraint.Values[i]}")).ToList();
            }

            yield return ($"{constraint.Rule}{constraint.Range2[0]}( {string.Join(", ", constrainedNames)} )", constraint.Description);
        }
    }

    private void HandleParameters(List<ParamBlock> paramBlocks, IndentedTextWriter writer)
    {
        foreach (var param in paramBlocks.OrderBy(x => x.Id == 255))
        {
            // BoolAttribute
            // FloatAttribute
            var attributes = new List<string>();

            // Render State
            if (param.ParamType is ParameterType.RenderState)
            {
                HandleState(writer, param);
            }
            // Sampler State
            else if (param.ParamType is ParameterType.SamplerState)
            {
                HandleState(writer, param);
            }

            // User input
            else if (param.Id == 255)
            {
                // Texture Input (unpacked)
                if (param.VecSize == -1)
                {
                    if (param.UiType != UiType.Texture)
                    {
                        throw new UnexpectedMagicException($"Expected {UiType.Texture}, got", (int)param.UiType, nameof(param.UiType));
                    }

                    if (param.ParamType != ParameterType.InputTexture)
                    {
                        throw new UnexpectedMagicException($"Expected {ParameterType.InputTexture}, got", (int)param.ParamType, nameof(param.ParamType));
                    }

                    var default4 = $"Default4({string.Join(", ", param.FloatDefs)})";

                    var mode = param.IntArgs1[2] == 0
                        ? "Linear"
                        : "Srgb";

                    var textureSuffix = param.Suffix.Length > 0
                        ? "_" + param.Suffix
                        : string.Empty;

                    writer.WriteLine($"CreateInputTexture2D({param.Name}, {mode}, {param.IntArgs1[3]}, \"{param.Command1}\", \"{textureSuffix}\", \"{param.UiGroup}\", {default4});");
                    // param.FileRef materials/default/default_cube.png
                    continue;
                }


                var intDefsCutOff = 0;
                var floatDefsCutOff = 0;
                for (var i = 3; i >= 0; i--)
                {
                    if (param.IntDefs[i] == 0)
                    {
                        intDefsCutOff++;
                    }

                    if (param.FloatDefs[i] == 0f)
                    {
                        floatDefsCutOff++;
                    }
                }

                static string GetFuncName(string func, int cutOff)
                    => cutOff == 3 ? func : func + (4 - cutOff);

                if (intDefsCutOff <= 3)
                {
                    if (floatDefsCutOff <= 3)
                    {
                        attributes.Add($"{GetFuncName("Default", floatDefsCutOff)}({string.Join(", ", param.FloatDefs[..^floatDefsCutOff])});");
                    }
                    else
                    {
                        var funcName = intDefsCutOff == 3 ? "Default" : "Default" + (4 - intDefsCutOff);
                        attributes.Add($"{GetFuncName("Default", intDefsCutOff)}({string.Join(", ", param.IntDefs[..^intDefsCutOff])});");
                    }
                }

                var intRangeCutOff = 0;
                var floatRangeCutOff = 0;
                for (var i = 3; i >= 0; i--)
                {
                    if (param.IntMins[i] == 0 && param.IntMaxs[i] == 1)
                    {
                        intRangeCutOff++;
                    }

                    if (param.FloatMins[i] == 0f && param.FloatMaxs[i] == 1f)
                    {
                        floatRangeCutOff++;
                    }
                }

                if (intRangeCutOff <= 3 && param.IntMins[0] != -ParamBlock.IntInf)
                {
                    if (floatRangeCutOff <= 3 && param.FloatMins[0] != -ParamBlock.FloatInf)
                    {
                        attributes.Add($"{GetFuncName("Range", floatRangeCutOff)}({string.Join(", ", param.FloatMins[..^floatRangeCutOff])}, {string.Join(", ", param.FloatMaxs[..^floatRangeCutOff])})");
                    }
                    else
                    {
                        attributes.Add($"{GetFuncName("Range", intRangeCutOff)}({string.Join(", ", param.IntMins[..^intRangeCutOff])}, {string.Join(", ", param.IntMaxs[..^intRangeCutOff])});");
                    }
                }

                if (param.AttributeName.Length > 0)
                {
                    attributes.Add($"Attribute(\"{param.AttributeName}\");");
                }

                if (param.UiType != UiType.None)
                {
                    attributes.Add($"UiType({param.UiType});");
                }

                if (param.UiGroup.Length > 0)
                {
                    attributes.Add($"UiGroup(\"{param.UiGroup}\");");
                }

                if (param.DynExp.Length > 0)
                {
                    var globals = paramBlocks.Select(p => p.Name).ToArray();
                    var dynEx = new VfxEval(param.DynExp, globals, omitReturnStatement: true, FeatureNames).DynamicExpressionResult;
                    dynEx = dynEx.Replace(param.Name, "this", StringComparison.Ordinal);
                    attributes.Add($"Expression({dynEx});");
                }

                var attributesVfx = attributes.Count > 0
                    ? " < " + string.Join(" ", attributes) + " > "
                    : string.Empty;

                writer.WriteLine($"{Vfx.Types.GetValueOrDefault(param.VfxType, $"unkntype{param.VfxType}")} {param.Name}{attributesVfx};");
            }
            else
            {

            }
        }
    }

    private void HandleState(IndentedTextWriter writer, ParamBlock param)
    {
        byte result = 255;
        var exists = param.ParamType switch
        {
            ParameterType.RenderState => Enum.TryParse(param.Name, false, out Unsafe.As<byte, RenderState>(ref result)),
            ParameterType.SamplerState => Enum.TryParse(param.Name, false, out Unsafe.As<byte, SamplerState>(ref result)),
            _ => throw new InvalidOperationException($"Expected {ParameterType.RenderState} or {ParameterType.SamplerState}, got {nameof(ParameterType)}={param.ParamType}"),
        };

        if (!exists || (exists && result != param.Id))
        {
            Console.WriteLine($"{param.Name} = {param.Id},");
        }

        var stateValue = param.DynExp.Length > 0
            ? new VfxEval(param.DynExp, Globals, omitReturnStatement: true, FeatureNames).DynamicExpressionResult
            : param.IntDefs[0].ToString(CultureInfo.InvariantCulture);

        if (param.ParamType == ParameterType.RenderState)
        {
            writer.WriteLine("{0}({1}, {2});", param.ParamType, param.Name, stateValue);
        }
        else
        {
            writer.WriteLine("{0}({1});", param.Name, stateValue);
        }
    }

    private string GetChannelFromMipmap(MipmapBlock mipmapBlock)
    {
        var mode = mipmapBlock.ColorMode == 0
            ? "Linear"
            : "Srgb";

        var cutoff = Array.IndexOf(mipmapBlock.InputTextureIndices, -1);
        var inputs = string.Join(", ", mipmapBlock.InputTextureIndices[..cutoff].Select(idx => Features.ParamBlocks[idx].Name));

        return $"Channel( {mipmapBlock.Channel}, {mipmapBlock.Name}( {inputs} ) );";
    }
}
