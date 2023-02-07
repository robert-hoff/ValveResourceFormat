using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;

namespace ValveResourceFormat.IO;

public sealed class ShaderExtract
{
    public ShaderFile Features { get; init; }
    public ShaderFile Vs { get; init; }
    public ShaderFile Ps { get; init; }

    public ShaderExtract(ShaderFile[] shaders)
    {
        Features = shaders[0];
    }

    public ShaderExtract(SortedDictionary<(VcsProgramType, string), ShaderFile> shaderCollection)
    {
        foreach (var shader in shaderCollection)
        {
            if (shader.Key.Item1 == VcsProgramType.Features)
            {
                Features = shader.Value;
            }

            if (shader.Key.Item1 == VcsProgramType.VertexShader)
            {
                Vs = shader.Value;
            }

            if (shader.Key.Item1 == VcsProgramType.PixelShader)
            {
                Ps = shader.Value;
            }
        }
    }

    public ContentFile ToContentFile()
    {
        var vfx = new ContentFile
        {
            Data = Encoding.UTF8.GetBytes(ToVFX())
        };

        return vfx;
    }

    public string ToVFX()
    {
        // TODO: IndentedTextWriter

        var headerSb = new StringBuilder();
        {
            headerSb.AppendLine("HEADER");
            headerSb.AppendLine("{");

            headerSb.AppendLine($"\tDescription = \"{Features.FeaturesHeader.FileDescription}\";");
            headerSb.AppendLine($"\tDevShader = {(Features.FeaturesHeader.DevShader == 0 ? "false" : "true")};");
            headerSb.AppendLine($"\tVersion = {Features.FeaturesHeader.Version};");

            headerSb.AppendLine("}");
        }

        var modesSb = new StringBuilder();
        {
            modesSb.AppendLine();
            modesSb.AppendLine("MODES");
            modesSb.AppendLine("{");

            foreach (var mode in Features.FeaturesHeader.Modes)
            {
                if (string.IsNullOrEmpty(mode.Shader))
                {
                    modesSb.AppendLine($"\t{mode.Name}({mode.StaticConfig});");
                }
                else
                {
                    modesSb.AppendLine($"\t{mode.Name}(\"{mode.Shader}\");");
                }
            }

            modesSb.AppendLine("}");
        }

        var featuresSb = new StringBuilder();
        {
            featuresSb.AppendLine();
            featuresSb.AppendLine("FEATURES");
            featuresSb.AppendLine("{");

            HandleFeatures(Features.SfBlocks, Features.SfConstraintsBlocks, featuresSb);

            featuresSb.AppendLine("}");
        }

        var commonSb = new StringBuilder();
        {
            commonSb.AppendLine();
            commonSb.AppendLine("COMMON");
            commonSb.AppendLine("{");

            if (Vs is not null && Vs.SymbolBlocks.Count > 0)
            {
                commonSb.AppendLine("\tstruct VS_INPUT");
                commonSb.AppendLine("\t{");

                foreach (var symbol in Vs.SymbolBlocks[0].SymbolsDefinition)
                {
                    var attributeVfx = symbol.Option.Length > 0 ? $" < Semantic({symbol.Option}); >" : string.Empty;
                    // TODO: type
                    commonSb.AppendLine($"\t\tfloat4 {symbol.Name} : {symbol.Type}{symbol.SemanticIndex}{attributeVfx};");
                }
            }

            commonSb.AppendLine("\t};");

            commonSb.AppendLine();

            commonSb.AppendLine("\tstruct PS_INPUT");
            commonSb.AppendLine("\t{");
            commonSb.AppendLine("\t\t//");
            commonSb.AppendLine("\t};");


            commonSb.AppendLine("}");
        }

        var features = Features.SfBlocks.Select(f => f.Name).ToList();

        // vs
        var vsSb = new StringBuilder();
        {
            vsSb.AppendLine();
            vsSb.AppendLine("VS");
            vsSb.AppendLine("{");
            if (Vs is not null)
            {
                HandleStaticCombos(Vs.SfBlocks, Vs.SfConstraintsBlocks, features, vsSb);
                HandleParameters(Vs.ParamBlocks, vsSb);
            }
            vsSb.AppendLine("}");
        }

        // ps
        var psSb = new StringBuilder();
        psSb.AppendLine();
        psSb.AppendLine("PS");
        psSb.AppendLine("{");

        if (Ps is not null)
        {
            HandleStaticCombos(Ps.SfBlocks, Ps.SfConstraintsBlocks, features, psSb);
            HandleParameters(Ps.ParamBlocks, psSb);
        }

        psSb.AppendLine("}");

        return headerSb.ToString() + modesSb.ToString() + featuresSb.ToString() + commonSb.ToString() + vsSb.ToString() + psSb.ToString();
    }

    private static void HandleFeatures(List<SfBlock> features, List<SfConstraintsBlock> constraints, StringBuilder sb)
    {
        foreach (var feature in features)
        {
            var checkboxNames = feature.CheckboxNames.Count > 0
                ? " (" + string.Join(", ", feature.CheckboxNames.Select((x, i) => $"{i}=\"{x}\"")) + ")"
                : string.Empty;

            // Verify RangeMin
            sb.AppendLine($"\tFeature( {feature.Name}, {feature.RangeMin}..{feature.RangeMax}{checkboxNames}, \"{feature.Category}\" );");
        }

        foreach (var rule in HandleConstraints(features, constraints))
        {
            sb.AppendLine($"\tFeatureRule( {rule.Constraint}, \"{rule.Description}\" );");
        }
    }
    private static void HandleStaticCombos(List<SfBlock> combos, List<SfConstraintsBlock> constraints, List<string> features, StringBuilder sb)
    {
        foreach (var staticCombo in combos)
        {
            if (staticCombo.FeatureIndex != -1)
            {
                sb.AppendLine($"\tStaticCombo( {staticCombo.Name}, {features[staticCombo.FeatureIndex]}, Sys( All ) );");
                continue;
            }

            sb.AppendLine($"\tStaticCombo( {staticCombo.Name}, {staticCombo.RangeMin}..{staticCombo.RangeMax}, Sys( All ) );");
        }

        foreach (var rule in HandleConstraints(combos, constraints))
        {
            sb.AppendLine($"\tStaticComboRule( {rule.Constraint} );");
        }
    }

    private static IEnumerable<(string Constraint, string Description)> HandleConstraints(List<SfBlock> sfBlocks, List<SfConstraintsBlock> constraints)
    {
        foreach (var constraint in constraints)
        {
            Console.WriteLine(string.Join(" ", constraint.Flags));
            var constrainedNames = string.Join(", ", constraint.Range0.Select(x => sfBlocks[x].Name));

            var rules = new List<string>
            {
                "<rule0>",
                "Requires1",
                "Requiress", // spritecard: FeatureRule(Requiress(F_NORMAL_MAP, F_TEXTURE_LAYERS, F_TEXTURE_LAYERS, F_TEXTURE_LAYERS), "Normal map requires Less than 3 Layers due to DX9");
                "Allow1",
            };

            yield return ($"{rules[constraint.RelRule]}( {constrainedNames} )", constraint.Description);
        }
    }

    private void HandleParameters(List<ParamBlock> paramBlocks, StringBuilder sb)
    {
        foreach (var param in paramBlocks.OrderBy(x => x.RenderState == 255))
        {
            // BoolAttribute
            // FloatAttribute
            var attributes = new List<string>();

            // Render State
            if (param.Arg2 == 5)
            {
                if (Enum.TryParse<RenderState>(param.Name, false, out var result))
                {
                    if ((byte)result != param.RenderState)
                    {
                        Console.WriteLine($"{param.Name} = {param.RenderState},");
                    }
                }
                else
                {
                    Console.WriteLine($"{param.Name} = {param.RenderState},");
                }

                if (param.DynExp.Length > 0)
                {
                    var features = Features.SfBlocks.Select(f => f.Name).ToList();
                    var globalVars = Features.ParamBlocks.Select(p => p.Name).ToArray();
                    var dynEx = new VfxEval(param.DynExp, globalVars, omitReturnStatement: true, features: features).DynamicExpressionResult;

                    sb.AppendLine($"\tRenderState({param.Name}, {dynEx});");
                }
                else
                {
                    sb.AppendLine($"\tRenderState({param.Name}, {param.IntDefs[0]});");
                }
            }

            // Sampler State
            else if (param.Arg2 == 6)
            {
                if (Enum.TryParse<SamplerState>(param.Name, false, out var result))
                {
                    if ((byte)result != param.RenderState)
                    {
                        Console.WriteLine($"{param.Name} = {param.RenderState},");
                    }
                }
                else
                {
                    Console.WriteLine($"{param.Name} = {param.RenderState},");
                }

                sb.AppendLine($"\t{param.Name}({param.IntDefs[0]}); // Sampler");
            }

            // User input
            else if (param.RenderState == 255)
            {
                // Texture Input (unpacked)
                if (param.Arg4 == -1)
                {
                    if (param.UiType != UiType.Texture)
                    {
                        throw new Exception("Unexpected UiType: " + param.UiType.ToString());
                    }

                    var default4 = $"Default4({param.FloatDefs[0]}, {param.FloatDefs[1]}, {param.FloatDefs[2]}, {param.FloatDefs[3]})";
                    var mode = param.IntArgs1[2] == 0
                        ? "Linear"
                        : "Srgb";
                    sb.AppendLine($"\tCreateInputTexture2D({param.Name}, {mode}, {param.IntArgs1[3]}, \"{param.Command1}\", \"{param.Command0}\", \"{param.UiGroup}\", {default4});");
                    // param.FileRef materials/default/default_cube.png
                    continue;
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
                    var features = Features.SfBlocks.Select(f => f.Name).ToList();
                    var globalVars = Ps.ParamBlocks.Select(p => p.Name).ToArray();
                    var dynEx = new VfxEval(param.DynExp, globalVars, omitReturnStatement: true, features: features).DynamicExpressionResult.Replace(param.Name, "this");
                    attributes.Add($"Expression({dynEx});");
                }

                var attributesVfx = attributes.Count > 0
                    ? " < " + string.Join(" ", attributes) + " > "
                    : string.Empty;

                sb.AppendLine($"\t{Vfx.Types.GetValueOrDefault(param.VfxType, $"unkntype{param.VfxType}")} {param.Name}{attributesVfx};");
            }
            else
            {

            }
        }
    }
}
