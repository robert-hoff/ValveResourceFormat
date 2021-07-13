using System;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.ParticleRenderer.Initializers {
    public class RandomAlpha : IParticleInitializer {
        private readonly int alphaMin = 255;
        private readonly int alphaMax = 255;

        private readonly Random random;

        public RandomAlpha(IKeyValueCollection keyValue) {
            random = new Random();

            if (keyValue.ContainsKey("m_nAlphaMin")) {
                alphaMin = (int)keyValue.GetIntegerProperty("m_nAlphaMin");
            }

            if (keyValue.ContainsKey("m_nAlphaMax")) {
                alphaMax = (int)keyValue.GetIntegerProperty("m_nAlphaMax");
            }

            if (alphaMin > alphaMax) {
                var temp = alphaMin;
                alphaMin = alphaMax;
                alphaMax = temp;
            }
        }

        public Particle Initialize(ref Particle particle, ParticleSystemRenderState particleSystemState) {
            var alpha = random.Next(alphaMin, alphaMax) / 255f;

            particle.ConstantAlpha = alpha;
            particle.Alpha = alpha;

            return particle;
        }
    }
}
