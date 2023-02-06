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
            headerSb.AppendLine();
            headerSb.AppendLine("HEADER");
            headerSb.AppendLine("{");

            headerSb.AppendLine($"\tDescription = \"{Features.FeaturesHeader.FileDescription}\";");
            headerSb.AppendLine($"\tDevShader = {(Features.FeaturesHeader.DevShader == 0 ? "false" : "true")};");
            headerSb.AppendLine($"\tVersion = {Features.FeaturesHeader.Version};");
            headerSb.AppendLine($"\tCompileTargets = (<unknown>);");

            headerSb.AppendLine("}");
        }

        var modesSb = new StringBuilder();
        {
            modesSb.AppendLine();
            modesSb.AppendLine("MODES");
            modesSb.AppendLine("{");

            foreach ((var mode, var condition) in Features.FeaturesHeader.MainParams)
            {
                modesSb.AppendLine($"\t{mode}({condition});");
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

            commonSb.AppendLine("\tstruct VS_INPUT");
            commonSb.AppendLine("\t{");

            foreach (var symbol in Vs.SymbolBlocks[0].SymbolsDefinition)
            {
                var attributeVfx = symbol.Option.Length > 0 ? $" < Semantic({symbol.Option}); >" : string.Empty;
                // TODO: type
                commonSb.AppendLine($"\t\tfloat4 {symbol.Name} : {symbol.Type}{symbol.SemanticIndex}{attributeVfx};");
            }

            commonSb.AppendLine("\t};");

            commonSb.AppendLine();

            commonSb.AppendLine("\tstruct PS_INPUT");
            commonSb.AppendLine("\t{");
            commonSb.AppendLine("\t\t//");
            commonSb.AppendLine("\t{");


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
        {
            psSb.AppendLine();
            psSb.AppendLine("PS");
            psSb.AppendLine("{");

            foreach (var param in Ps.ParamBlocks)
            {
                var attributes = new List<string>();
                if (param.Lead0 == 6 || param.Lead0 == 7)
                {
                    var dynEx = new VfxEval(param.DynExp, omitReturnStatement: true, features: Features.SfBlocks.Select(f => f.Name).ToList()).DynamicExpressionResult;
                    psSb.AppendLine($"\tRenderState({param.Name}, {dynEx});");
                    // BoolAttribute
                    // FloatAttribute
                    // <Expression();>
                }
                else
                {
                    if (param.UiType != UiType.None)
                    {
                        attributes.Add($"UiType({param.UiType});");
                    }

                    if (param.UiGroup.Length > 0)
                    {
                        attributes.Add($"UiGroup(\"{param.UiGroup}\");");
                    }

                    var attributesVfx = attributes.Count > 0
                        ? " < " + string.Join(" ", attributes) + " > "
                        : string.Empty;

                    psSb.AppendLine($"\t{Vfx.Types.GetValueOrDefault(param.Arg1, $"unkntype{param.Arg1}")} {param.Name}{attributesVfx};");
                }
            }


            psSb.AppendLine("}");
        }

        return headerSb.ToString() + modesSb.ToString() + featuresSb.ToString() + commonSb.ToString() + vsSb.ToString() + psSb.ToString();
    }
}
