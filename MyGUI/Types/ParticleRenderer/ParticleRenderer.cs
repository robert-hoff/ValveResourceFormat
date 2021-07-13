using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MyGUI.Types.ParticleRenderer.Emitters;
using MyGUI.Types.ParticleRenderer.Initializers;
using MyGUI.Types.ParticleRenderer.Operators;
using MyGUI.Types.ParticleRenderer.Renderers;
using MyGUI.Types.Renderer;
using MyGUI.Utils;
using MyValveResourceFormat.ResourceTypes;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.ParticleRenderer
{
    internal class ParticleRenderer : IRenderer
    {
        public IEnumerable<IParticleEmitter> Emitters { get; private set; } = new List<IParticleEmitter>();

        public IEnumerable<IParticleInitializer> Initializers { get; private set; } = new List<IParticleInitializer>();

        public IEnumerable<IParticleOperator> Operators { get; private set; } = new List<IParticleOperator>();

        public IEnumerable<IParticleRenderer> Renderers { get; private set; } = new List<IParticleRenderer>();

        public AABB BoundingBox { get; private set; }

        public Vector3 Position
        {
            get => systemRenderState.GetControlPoint(0);
            set
            {
                systemRenderState.SetControlPoint(0, value);
                foreach (var child in childParticleRenderers)
                {
                    child.Position = value;
                }
            }
        }

        private readonly List<ParticleRenderer> childParticleRenderers;
        private readonly VrfGuiContext vrfGuiContext;
        private bool hasStarted;

        private ParticleBag particleBag;
        private int particlesEmitted;
        private ParticleSystemRenderState systemRenderState;

        // TODO: Passing in position here was for testing, do it properly
        public ParticleRenderer(ParticleSystem particleSystem, VrfGuiContext vrfGuiContext, Vector3 pos = default)
        {
            childParticleRenderers = new List<ParticleRenderer>();
            this.vrfGuiContext = vrfGuiContext;

            particleBag = new ParticleBag(100, true);
            systemRenderState = new ParticleSystemRenderState();

            systemRenderState.SetControlPoint(0, pos);

            BoundingBox = new AABB(pos + new Vector3(-32, -32, -32), pos + new Vector3(32, 32, 32));

            SetupEmitters(particleSystem.Data, particleSystem.GetEmitters());
            SetupInitializers(particleSystem.GetInitializers());
            SetupOperators(particleSystem.GetOperators());
            SetupRenderers(particleSystem.GetRenderers());

            SetupChildParticles(particleSystem.GetChildParticleNames(true));
        }

        public void Start()
        {
            foreach (var emitter in Emitters)
            {
                emitter.Start(EmitParticle);
            }

            foreach (var childParticleRenderer in childParticleRenderers)
            {
                childParticleRenderer.Start();
            }
        }

        private void EmitParticle()
        {
            int index = particleBag.Add();
            if (index < 0)
            {
                Console.WriteLine("Out of space in particle bag");
                return;
            }

            particleBag.LiveParticles[index].ParticleCount = particlesEmitted++;
            InitializeParticle(ref particleBag.LiveParticles[index]);
        }

        private void InitializeParticle(ref Particle p)
        {
            p.Position = systemRenderState.GetControlPoint(0);

            foreach (var initializer in Initializers)
            {
                initializer.Initialize(ref p, systemRenderState);
            }
        }

        public void Stop()
        {
            foreach (var emitter in Emitters)
            {
                emitter.Stop();
            }

            foreach (var childParticleRenderer in childParticleRenderers)
            {
                childParticleRenderer.Stop();
            }
        }

        public void Restart()
        {
            Stop();
            systemRenderState.Lifetime = 0;
            particleBag.Clear();
            Start();

            foreach (var childParticleRenderer in childParticleRenderers)
            {
                childParticleRenderer.Restart();
            }
        }

        public void Update(float frameTime)
        {
            if (!hasStarted)
            {
                Start();
                hasStarted = true;
            }

            systemRenderState.Lifetime += frameTime;

            foreach (var emitter in Emitters)
            {
                emitter.Update(frameTime);
            }

            foreach (var particleOperator in Operators)
            {
                particleOperator.Update(particleBag.LiveParticles, frameTime, systemRenderState);
            }

            // Remove all dead particles
            particleBag.PruneExpired();

            var center = systemRenderState.GetControlPoint(0);
            if (particleBag.Count == 0)
            {
                BoundingBox = new AABB(center, center);
            }
            else
            {
                var minParticlePos = center;
                var maxParticlePos = center;

                var liveParticles = particleBag.LiveParticles;
                for (int i = 0; i < liveParticles.Length; ++i)
                {
                    var pos = liveParticles[i].Position;
                    var radius = liveParticles[i].Radius;
                    minParticlePos = Vector3.Min(minParticlePos, pos - new Vector3(radius));
                    maxParticlePos = Vector3.Max(maxParticlePos, pos + new Vector3(radius));
                }

                BoundingBox = new AABB(minParticlePos, maxParticlePos);
            }

            foreach (var childParticleRenderer in childParticleRenderers)
            {
                childParticleRenderer.Update(frameTime);
                BoundingBox = BoundingBox.Union(childParticleRenderer.BoundingBox);
            }

            // Restart if all emitters are done and all particles expired
            if (IsFinished())
            {
                Restart();
            }
        }

        public bool IsFinished()
            => Emitters.All(e => e.IsFinished)
            && particleBag.Count == 0
            && childParticleRenderers.All(r => r.IsFinished());

        public void Render(Camera camera, RenderPass renderPass)
        {
            if (particleBag.Count == 0)
            {
                return;
            }

            if (renderPass == RenderPass.Translucent || renderPass == RenderPass.Both)
            {
                foreach (var renderer in Renderers)
                {
                    renderer.Render(particleBag, camera.ViewProjectionMatrix, camera.CameraViewMatrix);
                }
            }

            foreach (var childParticleRenderer in childParticleRenderers)
            {
                childParticleRenderer.Render(camera, RenderPass.Both);
            }
        }

        private void SetupEmitters(IKeyValueCollection baseProperties, IEnumerable<IKeyValueCollection> emitterData)
        {
            var emitters = new List<IParticleEmitter>();

            foreach (var emitterInfo in emitterData)
            {
                var emitterClass = emitterInfo.GetProperty<string>("_class");
                if (ParticleControllerFactory.TryCreateEmitter(emitterClass, baseProperties, emitterInfo, out var emitter))
                {
                    emitters.Add(emitter);
                }
                else
                {
                    Console.WriteLine($"Unsupported emitter class '{emitterClass}'.");
                }
            }

            Emitters = emitters;
        }

        private void SetupInitializers(IEnumerable<IKeyValueCollection> initializerData)
        {
            var initializers = new List<IParticleInitializer>();

            foreach (var initializerInfo in initializerData)
            {
                var initializerClass = initializerInfo.GetProperty<string>("_class");
                if (ParticleControllerFactory.TryCreateInitializer(initializerClass, initializerInfo, out var initializer))
                {
                    initializers.Add(initializer);
                }
                else
                {
                    Console.WriteLine($"Unsupported initializer class '{initializerClass}'.");
                }
            }

            Initializers = initializers;
        }

        private void SetupOperators(IEnumerable<IKeyValueCollection> operatorData)
        {
            var operators = new List<IParticleOperator>();

            foreach (var operatorInfo in operatorData)
            {
                var operatorClass = operatorInfo.GetProperty<string>("_class");
                if (ParticleControllerFactory.TryCreateOperator(operatorClass, operatorInfo, out var @operator))
                {
                    operators.Add(@operator);
                }
                else
                {
                    Console.WriteLine($"Unsupported operator class '{operatorClass}'.");
                }
            }

            Operators = operators;
        }

        private void SetupRenderers(IEnumerable<IKeyValueCollection> rendererData)
        {
            var renderers = new List<IParticleRenderer>();

            foreach (var rendererInfo in rendererData)
            {
                var rendererClass = rendererInfo.GetProperty<string>("_class");
                if (ParticleControllerFactory.TryCreateRender(rendererClass, rendererInfo, vrfGuiContext, out var renderer))
                {
                    renderers.Add(renderer);
                }
                else
                {
                    Console.WriteLine($"Unsupported renderer class '{rendererClass}'.");
                }
            }

            Renderers = renderers;
        }

        private void SetupChildParticles(IEnumerable<string> childNames)
        {
            foreach (var childName in childNames)
            {
                var childResource = vrfGuiContext.LoadFileByAnyMeansNecessary(childName + "_c");
                var childSystem = (ParticleSystem)childResource.DataBlock;

                childParticleRenderers.Add(new ParticleRenderer(childSystem, vrfGuiContext, systemRenderState.GetControlPoint(0)));
            }
        }
    }
}
