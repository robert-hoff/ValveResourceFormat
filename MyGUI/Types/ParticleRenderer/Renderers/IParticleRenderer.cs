using System;
using System.Numerics;

namespace MyGUI.Types.ParticleRenderer.Renderers {
    public interface IParticleRenderer {
        void Render(ParticleBag particles, Matrix4x4 viewProjectionMatrix, Matrix4x4 modelViewMatrix);
    }
}
