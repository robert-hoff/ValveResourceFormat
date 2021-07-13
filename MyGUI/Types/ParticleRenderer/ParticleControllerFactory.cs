using System;
using System.Collections.Generic;
using MyGUI.Types.ParticleRenderer.Emitters;
using MyGUI.Types.ParticleRenderer.Initializers;
using MyGUI.Types.ParticleRenderer.Operators;
using MyGUI.Types.ParticleRenderer.Renderers;
using MyGUI.Utils;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.ParticleRenderer {
    public static class ParticleControllerFactory {
        // Register particle emitters
        private static readonly IDictionary<string, Func<IKeyValueCollection, IKeyValueCollection, IParticleEmitter>> EmitterDictionary
            = new Dictionary<string, Func<IKeyValueCollection, IKeyValueCollection, IParticleEmitter>> {
                ["C_OP_InstantaneousEmitter"] = (baseProperties, emitterInfo) => new InstantaneousEmitter(baseProperties, emitterInfo),
                ["C_OP_ContinuousEmitter"] = (baseProperties, emitterInfo) => new ContinuousEmitter(baseProperties, emitterInfo),
            };

        // Register particle initializers
        private static readonly IDictionary<string, Func<IKeyValueCollection, IParticleInitializer>> InitializerDictionary
            = new Dictionary<string, Func<IKeyValueCollection, IParticleInitializer>> {
                ["C_INIT_CreateWithinSphere"] = initializerInfo => new CreateWithinSphere(initializerInfo),
                ["C_INIT_InitialVelocityNoise"] = initializerInfo => new InitialVelocityNoise(initializerInfo),
                ["C_INIT_OffsetVectorToVector"] = initializerInfo => new OffsetVectorToVector(initializerInfo),
                ["C_INIT_PositionOffset"] = initializerInfo => new PositionOffset(initializerInfo),
                ["C_INIT_RandomAlpha"] = initializerInfo => new RandomAlpha(initializerInfo),
                ["C_INIT_RandomColor"] = initializerInfo => new RandomColor(initializerInfo),
                ["C_INIT_RandomLifeTime"] = initializerInfo => new RandomLifeTime(initializerInfo),
                ["C_INIT_RandomRadius"] = initializerInfo => new RandomRadius(initializerInfo),
                ["C_INIT_RandomRotation"] = initializerInfo => new RandomRotation(initializerInfo),
                ["C_INIT_RandomRotationSpeed"] = initializerInfo => new RandomRotationSpeed(initializerInfo),
                ["C_INIT_RandomSequence"] = initializerInfo => new RandomSequence(initializerInfo),
                ["C_INIT_RandomTrailLength"] = initializerInfo => new RandomTrailLength(initializerInfo),
                ["C_INIT_RemapParticleCountToScalar"] = initializerInfo => new RemapParticleCountToScalar(initializerInfo),
                ["C_INIT_RingWave"] = initializerInfo => new RingWave(initializerInfo),
            };

        // Register particle operators
        private static readonly IDictionary<string, Func<IKeyValueCollection, IParticleOperator>> OperatorDictionary
            = new Dictionary<string, Func<IKeyValueCollection, IParticleOperator>> {
                ["C_OP_Decay"] = operatorInfo => new Decay(operatorInfo),
                ["C_OP_BasicMovement"] = operatorInfo => new BasicMovement(operatorInfo),
                ["C_OP_ColorInterpolate"] = operatorInfo => new ColorInterpolate(operatorInfo),
                ["C_OP_InterpolateRadius"] = operatorInfo => new InterpolateRadius(operatorInfo),
                ["C_OP_FadeAndKill"] = operatorInfo => new FadeAndKill(operatorInfo),
                ["C_OP_FadeInSimple"] = operatorInfo => new FadeInSimple(operatorInfo),
                ["C_OP_FadeOutSimple"] = operatorInfo => new FadeOutSimple(operatorInfo),
                ["C_OP_OscillateScalar"] = operatorInfo => new OscillateScalar(operatorInfo),
                ["C_OP_SpinUpdate"] = operatorInfo => new SpinUpdate(operatorInfo),
            };

        // Register particle renderers
        private static readonly IDictionary<string, Func<IKeyValueCollection, VrfGuiContext, IParticleRenderer>> RendererDictionary
            = new Dictionary<string, Func<IKeyValueCollection, VrfGuiContext, IParticleRenderer>> {
                ["C_OP_RenderSprites"] = (rendererInfo, vrfGuiContext) => new RenderSprites(rendererInfo, vrfGuiContext),
                ["C_OP_RenderTrails"] = (rendererInfo, vrfGuiContext) => new RenderTrails(rendererInfo, vrfGuiContext),
            };

        public static bool TryCreateEmitter(string name, IKeyValueCollection baseProperties, IKeyValueCollection emitterInfo, out IParticleEmitter emitter) {
            if (EmitterDictionary.TryGetValue(name, out var factory)) {
                emitter = factory(baseProperties, emitterInfo);
                return true;
            }

            emitter = default;
            return false;
        }

        public static bool TryCreateInitializer(string name, IKeyValueCollection initializerInfo, out IParticleInitializer initializer) {
            if (InitializerDictionary.TryGetValue(name, out var factory)) {
                initializer = factory(initializerInfo);
                return true;
            }

            initializer = default;
            return false;
        }

        public static bool TryCreateOperator(string name, IKeyValueCollection operatorInfo, out IParticleOperator @operator) {
            if (OperatorDictionary.TryGetValue(name, out var factory)) {
                @operator = factory(operatorInfo);
                return true;
            }

            @operator = default;
            return false;
        }

        public static bool TryCreateRender(string name, IKeyValueCollection rendererInfo, VrfGuiContext vrfGuiContext, out IParticleRenderer renderer) {
            if (RendererDictionary.TryGetValue(name, out var factory)) {
                renderer = factory(rendererInfo, vrfGuiContext);
                return true;
            }

            renderer = default;
            return false;
        }
    }
}
