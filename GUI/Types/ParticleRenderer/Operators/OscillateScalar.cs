using System;
using System.Collections.Generic;
using ValveResourceFormat.Serialization;

namespace GUI.Types.ParticleRenderer.Operators
{
    public class OscillateScalar : IParticleOperator
    {
        private readonly ParticleField outputField = ParticleField.Alpha;
        private readonly float rateMin;
        private readonly float rateMax;
        private readonly float frequencyMin = 1f;
        private readonly float frequencyMax = 1f;
        private readonly float oscillationMultiplier = 2f;
        private readonly float oscillationOffset = 0.5f;
        private readonly bool proportional = true;

        public OscillateScalar(IKeyValueCollection keyValues)
        {
            if (keyValues.ContainsKey("m_nField"))
            {
                outputField = (ParticleField)keyValues.GetIntegerProperty("m_nField");
            }

            if (keyValues.ContainsKey("m_RateMin"))
            {
                rateMin = keyValues.GetFloatProperty("m_RateMin");
            }

            if (keyValues.ContainsKey("m_RateMax"))
            {
                rateMax = keyValues.GetFloatProperty("m_RateMax");
            }

            if (keyValues.ContainsKey("m_FrequencyMin"))
            {
                frequencyMin = keyValues.GetFloatProperty("m_FrequencyMin");
            }

            if (keyValues.ContainsKey("m_FrequencyMax"))
            {
                frequencyMax = keyValues.GetFloatProperty("m_FrequencyMax");
            }

            if (keyValues.ContainsKey("m_flOscMult"))
            {
                oscillationMultiplier = keyValues.GetFloatProperty("m_flOscMult");
            }

            if (keyValues.ContainsKey("m_flOscAdd"))
            {
                oscillationOffset = keyValues.GetFloatProperty("m_flOscAdd");
            }

            if (keyValues.ContainsKey("m_bProportionalOp"))
            {
                proportional = keyValues.GetProperty<bool>("m_bProportionalOp");
            }
        }

        public void Update(Span<Particle> particles, float frameTime, ParticleSystemRenderState particleSystemState)
        {
            // Remove expired particles
            /*var particlesToRemove = particleRates.Keys.Except(particles[i]).ToList();
            foreach (var p in particlesToRemove)
            {
                particleRates.Remove(p);
                particleFrequencies.Remove(p);
            }*/

            // Update remaining particles
            for (var i = 0; i < particles.Length; ++i)
            {
                var rate = GetParticleRate(particles[i].ParticleCount);
                var frequency = GetParticleFrequency(particles[i].ParticleCount);

                var t = proportional
                    ? 1 - (particles[i].Lifetime / particles[i].ConstantLifetime)
                    : particles[i].Lifetime;

                var delta = (float)Math.Sin(((t * frequency * oscillationMultiplier) + oscillationOffset) * Math.PI);

                if (outputField == ParticleField.Radius)
                {
                    particles[i].Radius += delta * rate * frameTime;
                }
                else if (outputField == ParticleField.Alpha)
                {
                    particles[i].Alpha += delta * rate * frameTime;
                }
                else if (outputField == ParticleField.AlphaAlternate)
                {
                    particles[i].AlphaAlternate += delta * rate * frameTime;
                }
            }
        }

        private readonly Dictionary<int, float> particleRates = new();
        private readonly Dictionary<int, float> particleFrequencies = new();

        private float GetParticleRate(int particleId)
        {
            if (particleRates.TryGetValue(particleId, out var rate))
            {
                return rate;
            }
            else
            {
                var newRate = rateMin + ((float)Random.Shared.NextDouble() * (rateMax - rateMin));
                particleRates[particleId] = newRate;
                return newRate;
            }
        }

        private float GetParticleFrequency(int particleId)
        {
            if (particleFrequencies.TryGetValue(particleId, out var frequency))
            {
                return frequency;
            }
            else
            {
                var newFrequency = frequencyMin + ((float)Random.Shared.NextDouble() * (frequencyMax - frequencyMin));
                particleFrequencies[particleId] = newFrequency;
                return newFrequency;
            }
        }
    }
}
