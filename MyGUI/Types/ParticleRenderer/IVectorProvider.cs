using System;
using System.Numerics;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.ParticleRenderer {
    public interface IVectorProvider {
        Vector3 NextVector();
    }

    public class LiteralVectorProvider : IVectorProvider {
        private readonly Vector3 value;

        public LiteralVectorProvider(Vector3 value) {
            this.value = value;
        }

        public LiteralVectorProvider(double[] value) {
            this.value = new Vector3((float)value[0], (float)value[1], (float)value[2]);
        }

        public Vector3 NextVector() => value;
    }

    public static class IVectorProviderExtensions {
        public static IVectorProvider GetVectorProvider(this IKeyValueCollection keyValues, string propertyName) {
            var property = keyValues.GetProperty<object>(propertyName);

            if (property is IKeyValueCollection numberProviderParameters && numberProviderParameters.ContainsKey("m_nType")) {
                var type = numberProviderParameters.GetProperty<string>("m_nType");
                switch (type) {
                    case "PVEC_TYPE_LITERAL":
                        return new LiteralVectorProvider(numberProviderParameters.GetArray<double>("m_vLiteralValue"));
                    default:
                        if (numberProviderParameters.ContainsKey("m_vLiteralValue")) {
                            Console.Error.WriteLine($"Vector provider of type {type} is not directly supported, but it has m_vLiteralValue.");
                            return new LiteralVectorProvider(numberProviderParameters.GetArray<double>("m_vLiteralValue"));
                        }

                        throw new InvalidCastException($"Could not create vector provider of type {type}.");
                }
            }

            return new LiteralVectorProvider(keyValues.GetArray<double>(propertyName));
        }
    }
}
