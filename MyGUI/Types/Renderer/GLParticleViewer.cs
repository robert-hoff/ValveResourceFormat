using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;
using MyGUI.Controls;
using MyGUI.Types.ParticleRenderer;
using MyGUI.Utils;
using static MyGUI.Controls.GLViewerControl;

namespace MyGUI.Types.Renderer
{
    /// <summary>
    /// GL Render control with particle controls (control points? particle counts?).
    /// Renders a list of ParticleRenderers.
    /// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    internal class GLParticleViewer
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private ICollection<ParticleRenderer.ParticleRenderer> Renderers { get; } = new HashSet<ParticleRenderer.ParticleRenderer>();

        public event EventHandler Load;

        public Control Control => viewerControl;

        private readonly GLViewerControl viewerControl;
        private readonly VrfGuiContext vrfGuiContext;

        private ParticleGrid particleGrid;

        public GLParticleViewer(VrfGuiContext guiContext)
        {
            vrfGuiContext = guiContext;

            viewerControl = new GLViewerControl();

            viewerControl.GLLoad += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            particleGrid = new ParticleGrid(20, 5, vrfGuiContext);

            viewerControl.Camera.SetViewportSize(viewerControl.GLControl.Width, viewerControl.GLControl.Height);
            viewerControl.Camera.SetLocation(new Vector3(200));
            viewerControl.Camera.LookAt(new Vector3(0));

            Load?.Invoke(this, e);

            viewerControl.GLPaint += OnPaint;
        }

        private void OnPaint(object sender, RenderEventArgs e)
        {
            particleGrid.Render(e.Camera, RenderPass.Both);

            foreach (var renderer in Renderers)
            {
                renderer.Update(e.FrameTime);

                renderer.Render(e.Camera, RenderPass.Both);
            }
        }

        public void AddRenderer(ParticleRenderer.ParticleRenderer renderer)
        {
            Renderers.Add(renderer);
        }
    }
}
