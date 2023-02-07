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

            foreach (var sf in Features.SfBlocks)
            {
                var checkboxNames = sf.CheckboxNames.Count > 0
                    ? " (" + string.Join(", ", sf.CheckboxNames.Select((x, i) => $"{i}=\"{x}\"")) + ")"
                    : string.Empty;

                // Verify RangeMin
                featuresSb.AppendLine($"\tFeature({sf.Name}, {sf.RangeMin}..{sf.RangeMax}{checkboxNames}, \"{sf.Category}\");");
            }

            foreach (var sfConstraint in Features.SfConstraintsBlocks)
            {
                var constrainedFeatures = string.Join(", ", sfConstraint.Range0.Select(x => Features.SfBlocks[x].Name));

                var rules = new List<string>
                {
                    "<unknown0>",
                    "Requires",
                    "<unknown2>",
                    "Allow",
                };

                // Verify Arg0
                var rule = $"{rules[sfConstraint.RelRule]}{sfConstraint.Arg0}({constrainedFeatures})";
                featuresSb.AppendLine($"\tFeatureRule({rule}, \"{sfConstraint.Description}\");");
            }

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

        // vs
        var vsSb = new StringBuilder();
        {
            vsSb.AppendLine();
            vsSb.AppendLine("VS");
            vsSb.AppendLine("{");
            vsSb.AppendLine();
            vsSb.AppendLine("}");
        }

        // ps
        var psSb = new StringBuilder();
        psSb.AppendLine();
        psSb.AppendLine("PS");
        psSb.AppendLine("{");

        if (Ps is not null)
        {
            foreach (var param in Ps.ParamBlocks.OrderBy(x => x.RenderState == 255))
            {
                var attributes = new List<string>();
                //if (param.Lead0 == 6 || param.Lead0 == 7)
                //{
                //    var features = Features.SfBlocks.Select(f => f.Name).ToList();
                //    var globalVars = Ps.ParamBlocks.Select(p => p.Name).ToArray();
                //    var dynEx = new VfxEval(param.DynExp, globalVars, omitReturnStatement: true, features: features).DynamicExpressionResult;
                //    psSb.AppendLine($"\tRenderState({param.Name}, {dynEx});");
                //    // BoolAttribute
                //    // FloatAttribute
                //    // <Expression();>
                //}


                if (Enum.IsDefined(typeof(RenderState), (int)param.RenderState))
                {
                    if (param.DynExp.Length > 0)
                    {
                        var features = Features.SfBlocks.Select(f => f.Name).ToList();
                        var globalVars = Ps.ParamBlocks.Select(p => p.Name).ToArray();
                        var dynEx = new VfxEval(param.DynExp, globalVars, omitReturnStatement: true, features: features).DynamicExpressionResult;

                        if (param.Name != ((RenderState)param.RenderState).ToString())
                        {

                        }
                        psSb.AppendLine($"\tRenderState({param.Name}, {dynEx});");
                    }
                    else
                    {
                        psSb.AppendLine($"\tRenderState({param.Name}, unknownval!);");
                    }
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
                        psSb.AppendLine($"\tCreateInputTexture2D({param.Name}, {mode}, {param.IntArgs1[3]}, \"{param.Command1}\", \"{param.Command0}\", \"{param.UiGroup}\", {default4});");
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

                    psSb.AppendLine($"\t{Vfx.Types.GetValueOrDefault(param.Arg1, $"unkntype{param.Arg1}")} {param.Name}{attributesVfx};");
                }
                else
                {

                }
            }
        }
        psSb.AppendLine("}");

        return headerSb.ToString() + modesSb.ToString() + featuresSb.ToString() + commonSb.ToString() + vsSb.ToString() + psSb.ToString();
    }
}
