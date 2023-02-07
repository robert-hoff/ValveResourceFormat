
namespace ValveResourceFormat.CompiledShader;

[System.Flags]
public enum RenderState
{
    Filter = 0,
    AddressU = 1,
    AddressV = 2,
    AddressW = 3,
    BorderColor = 4,
    MaxAnisotropy = 5,
    MipLodBias = 6,
    MaxAniso = 7,
    MinLod = 8,
    MaxLod = 9,
    CullMode = 10,
    FillMode = 11,
    AlphaBlendEnable = 12,
    SrcBlend = 13,
    DestBlend = 14,
    BlendOp = 15,

    DepthEnable = 16,
    DepthWriteEnable = 17,
    DepthFunc = 18,
    StencilEnable = 21,
    StencilReadMask = 22,
    StencilWriteMask = 23,
    StencilFailOp = 24,
    StencilDepthFailOp = 25,
    StencilPassOp = 26,
    StencilFunc = 27,
    BackStencilFailOp = 28,
    BackStencilDepthFailOp = 29,
    BackStencilPassOp = 30,
    BackStencilFunc = 31,
    StencilRef = 32,
}
