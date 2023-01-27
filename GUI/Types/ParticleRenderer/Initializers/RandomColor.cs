using System;
using System.Numerics;
using ValveResourceFormat.Serialization;

namespace GUI.Types.ParticleRenderer.Initializers
{
    public class RandomColor : IParticleInitializer
    {
        private readonly Vector3 colorMin = Vector3.One;
        private readonly Vector3 colorMax = Vector3.One;

        public RandomColor(IKeyValueCollection keyValues)
        {
            if (keyValues.ContainsKey("m_ColorMin"))
            {
                var vectorValues = keyValues.GetIntegerArray("m_ColorMin");
                colorMin = new Vector3(vectorValues[0], vectorValues[1], vectorValues[2]) / 255f;
            }

            if (keyValues.ContainsKey("m_ColorMax"))
            {
                var vectorValues = keyValues.GetIntegerArray("m_ColorMax");
                colorMax = new Vector3(vectorValues[0], vectorValues[1], vectorValues[2]) / 255f;
            }
        }

        public Particle Initialize(ref Particle particle, ParticleSystemRenderState particleSystemState)
        {
            var t = (float)Random.Shared.NextDouble();
            particle.ConstantColor = colorMin + (t * (colorMax - colorMin));
            particle.Color = particle.ConstantColor;

            return particle;
        }
    }
}
