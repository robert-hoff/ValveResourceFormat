using System;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.ParticleRenderer.Operators
{
    public class FadeOutSimple : IParticleOperator
    {
        private readonly float fadeOutTime = 0.25f;

        public FadeOutSimple(IKeyValueCollection keyValues)
        {
            if (keyValues.ContainsKey("m_flFadeOutTime"))
            {
                fadeOutTime = keyValues.GetFloatProperty("m_flFadeOutTime");
            }
        }

        public void Update(Span<Particle> particles, float frameTime, ParticleSystemRenderState particleSystemState)
        {
            for (int i = 0; i < particles.Length; ++i)
            {
                var timeLeft = particles[i].Lifetime / particles[i].ConstantLifetime;
                if (timeLeft <= fadeOutTime)
                {
                    var t = timeLeft / fadeOutTime;
                    particles[i].Alpha = t * particles[i].ConstantAlpha;
                }
            }
        }
    }
}
