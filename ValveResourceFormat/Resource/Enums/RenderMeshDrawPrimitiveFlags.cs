using System;

namespace ValveResourceFormat
{
    [Flags]
    public enum RenderMeshDrawPrimitiveFlags
    {
        None = 0x0,
        UseShadowFastPath = 0x1,
        UseCompressedNormalTangent = 0x2,
        IsOccluder = 0x4,
        InputLayoutIsNotMatchedToMaterial = 0x8,
        HasBakedLightingFromVertexStream = 0x10,
        HasBakedLightingFromLightmap = 0x20,
        CanBatchWithDynamicShaderConstants = 0x40,
        DrawLast = 0x80,
        HasPerInstanceBakedLightingData = 0x100,
    }
}
