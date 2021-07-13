using System;

namespace MyGUI.Types.ParticleRenderer.Operators
{
    public interface IParticleOperator
    {
        void Update(Span<Particle> particles, float frameTime, ParticleSystemRenderState particleSystemState);
    }
}
