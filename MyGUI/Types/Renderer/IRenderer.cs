using System;

namespace MyGUI.Types.Renderer {
    internal interface IRenderer {
        AABB BoundingBox { get; }

        void Render(Camera camera, RenderPass renderPass);

        void Update(float frameTime);
    }
}
