using System;
using MyValveResourceFormat.ResourceTypes;

namespace MyGUI.Types.Renderer
{
    internal class ParticleSceneNode : SceneNode
    {
        private ParticleRenderer.ParticleRenderer particleRenderer;

        public ParticleSceneNode(Scene scene, ParticleSystem particleSystem)
            : base(scene)
        {
            particleRenderer = new ParticleRenderer.ParticleRenderer(particleSystem, Scene.GuiContext);
            LocalBoundingBox = particleRenderer.BoundingBox;
        }

        public override void Update(Scene.UpdateContext context)
        {
            particleRenderer.Position = Transform.Translation;
            particleRenderer.Update(context.Timestep);

            LocalBoundingBox = particleRenderer.BoundingBox.Translate(-particleRenderer.Position);
        }

        public override void Render(Scene.RenderContext context)
        {
            particleRenderer.Render(context.Camera, context.RenderPass);
        }
    }
}
