using System;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.ParticleRenderer.Initializers {
    public class RemapParticleCountToScalar : IParticleInitializer {
        private readonly long fieldOutput = 3;
        private readonly long inputMin;
        private readonly long inputMax = 10;
        private readonly float outputMin;
        private readonly float outputMax = 1f;
        private readonly bool scaleInitialRange;

        public RemapParticleCountToScalar(IKeyValueCollection keyValues) {
            if (keyValues.ContainsKey("m_nFieldOutput")) {
                fieldOutput = keyValues.GetIntegerProperty("m_nFieldOutput");
            }

            if (keyValues.ContainsKey("m_nInputMin")) {
                inputMin = keyValues.GetIntegerProperty("m_nInputMin");
            }

            if (keyValues.ContainsKey("m_nInputMax")) {
                inputMax = keyValues.GetIntegerProperty("m_nInputMax");
            }

            if (keyValues.ContainsKey("m_flOutputMin")) {
                outputMin = keyValues.GetIntegerProperty("m_flOutputMin");
            }

            if (keyValues.ContainsKey("m_flOutputMax")) {
                outputMax = keyValues.GetIntegerProperty("m_flOutputMax");
            }

            if (keyValues.ContainsKey("m_bScaleInitialRange")) {
                scaleInitialRange = keyValues.GetProperty<bool>("m_bScaleInitialRange");
            }
        }

        public Particle Initialize(ref Particle particle, ParticleSystemRenderState particleSystemState) {
            var particleCount = Math.Min(inputMax, Math.Max(inputMin, particle.ParticleCount));
            var t = (particleCount - inputMin) / (float)(inputMax - inputMin);

            var output = outputMin + (t * (outputMax - outputMin));

            switch (fieldOutput) {
                case 3:
                    particle.Radius = scaleInitialRange
                        ? particle.Radius * output
                        : output;
                    break;
            }

            return particle;
        }
    }
}
