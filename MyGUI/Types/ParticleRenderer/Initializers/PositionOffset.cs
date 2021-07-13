using System;
using System.Numerics;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.ParticleRenderer.Initializers
{
    public class PositionOffset : IParticleInitializer
    {
        private readonly Vector3 offsetMin = Vector3.Zero;
        private readonly Vector3 offsetMax = Vector3.Zero;

        private readonly Random random = new Random();

        public PositionOffset(IKeyValueCollection keyValues)
        {
            if (keyValues.ContainsKey("m_OffsetMin"))
            {
                var vectorValues = keyValues.GetArray<double>("m_OffsetMin");
                offsetMin = new Vector3((float)vectorValues[0], (float)vectorValues[1], (float)vectorValues[2]);
            }

            if (keyValues.ContainsKey("m_OffsetMax"))
            {
                var vectorValues = keyValues.GetArray<double>("m_OffsetMax");
                offsetMax = new Vector3((float)vectorValues[0], (float)vectorValues[1], (float)vectorValues[2]);
            }
        }

        public Particle Initialize(ref Particle particle, ParticleSystemRenderState particleSystemState)
        {
            var distance = offsetMax - offsetMin;
            var offset = offsetMin + (distance * new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble()));

            particle.Position += offset;

            return particle;
        }
    }
}
