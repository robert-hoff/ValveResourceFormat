using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.ThirdParty;

namespace TestVRF
{
    class CheckExternalVarsHashes
    {
        public const uint MURMUR2SEED = 0x31415926; // pi!

        public static void checkNames()
        {
            buildExternalVarsReference();

            foreach (KeyValuePair<uint, string> entry in externalVarsReference)
            {
                uint reference_val = entry.Key;
                uint murmur32 = MurmurHash2.Hash(entry.Value.ToLower(), MURMUR2SEED);
                Debug.WriteLine("murmur32 for {0} = {1:x8}", entry.Value, murmur32);

                if (reference_val != murmur32)
                {
                    throw new Exception("data mismatch!");
                }

            }
        }


        static Dictionary<uint, string> externalVarsReference = new Dictionary<uint, string>();
        public static void buildExternalVarsReference()
        {
            // Dota 2
            externalVarsReference.Add(0xd24b982f, "uiTexture");
            externalVarsReference.Add(0x8c954a0d, "fontTexture");
            externalVarsReference.Add(0xbaf50224, "scale1");
            externalVarsReference.Add(0x3b48bcd3, "scale2");
            externalVarsReference.Add(0x7dd532ad, "speed");
            externalVarsReference.Add(0x1ecf71e1, "a");
            externalVarsReference.Add(0xb7fdb72a, "b");
            externalVarsReference.Add(0x1df1849c, "intensity");
            externalVarsReference.Add(0x964485cc, "time");
            externalVarsReference.Add(0x336a0f0c, "$AGE");
            externalVarsReference.Add(0x1527c91c, "$ALPHA");
            externalVarsReference.Add(0xd772913d, "$TRANS_OFFSET_V");
            externalVarsReference.Add(0xa37a3e54, "$TRANS_SCALE_V");
            externalVarsReference.Add(0x25339664, "$OPACITY");
            externalVarsReference.Add(0x69e2f05e, "$TEX_COORD_OFFSET_U");
            externalVarsReference.Add(0x0a5b7f24, "$TEX_COORD_OFFSET_V");
            externalVarsReference.Add(0x7716a69a, "$PA_ARCANA_SPECULAR_BLOOM_SCALE");
            externalVarsReference.Add(0xd73c9c2f, "$PA_ARCANA_DETAIL1BLENDFACTOR");
            externalVarsReference.Add(0x287263fc, "$PA_ARCANA_DETAIL1SCALE");
            externalVarsReference.Add(0xd4147a1f, "$PA_ARCANA_DETAIL1TINT");
            externalVarsReference.Add(0xa58452dc, "$PA_ARCANA_SPECULAR_BLOOM_COLOR");
            externalVarsReference.Add(0x514616e6, "$GemColor");
            externalVarsReference.Add(0x9eac976a, "$overbright");
            externalVarsReference.Add(0xab2163a4, "$TEX_COLOR");
            externalVarsReference.Add(0x3225af29, "y");
            externalVarsReference.Add(0x84321e5f, "$DETAILBLEND");
            externalVarsReference.Add(0x276085fb, "FadeOut");
            externalVarsReference.Add(0xc4a1f8f7, "$COLOR");
            externalVarsReference.Add(0xf588a3d3, "$CLOAKINT");
            externalVarsReference.Add(0xda4e0212, "$SPIN");
            externalVarsReference.Add(0xb57746a1, "panoramaTexCoordOffset");
            externalVarsReference.Add(0xe244f4af, "panoramaTexCoordScale");
            externalVarsReference.Add(0x341f4361, "panoramaLayer");
            externalVarsReference.Add(0x1b927481, "avatarTexture");
            externalVarsReference.Add(0x57b2b714, "$ent_Health");
            externalVarsReference.Add(0x546a87df, "proceduralSprayTexture");
            externalVarsReference.Add(0x9d389d79, "alive");
            externalVarsReference.Add(0x46ec689a, "zz");
            externalVarsReference.Add(0x7068cf59, "aa");
            externalVarsReference.Add(0xff492a3a, "bb");
            externalVarsReference.Add(0xcb9a78d4, "cc");
            externalVarsReference.Add(0xde117c4a, "dd");
            externalVarsReference.Add(0xeb075669, "ee");
            externalVarsReference.Add(0x285fc55e, "ff");
            externalVarsReference.Add(0x39de2fbd, "FadeOut_blade");

            // HL Alyx
            externalVarsReference.Add(0xe7dc4bd6, "$BaseTexture");
            externalVarsReference.Add(0x30ee22ba, "colorAttrMovie");
            externalVarsReference.Add(0x98a42c96, "$DISSOLVE");
            externalVarsReference.Add(0x13e5d925, "$BRIGHTNESS");
            externalVarsReference.Add(0x097ad797, "logo_draw");
            externalVarsReference.Add(0xbad34216, "$SELFILLUM");
            externalVarsReference.Add(0xf8d95bff, "$SCROLLX");
            externalVarsReference.Add(0xfd912b88, "$SCROLLY");
            externalVarsReference.Add(0x0b2a3d85, "$EMISSIVEBRIGHTNESS");
            externalVarsReference.Add(0x41b948dc, "$EMISSIVESCALE");
            externalVarsReference.Add(0x8f3e65c3, "$NOISE");
            externalVarsReference.Add(0xd4db18d9, "$Emissive");
            externalVarsReference.Add(0x2797e0f8, "$Time");
            externalVarsReference.Add(0xa359c3d2, "$Enabled");
            externalVarsReference.Add(0x0a7ef0bc, "colorAttr");
            externalVarsReference.Add(0x52d9cda7, "$SPEED");
            externalVarsReference.Add(0x26b36985, "$FRESNEL");
            externalVarsReference.Add(0xd7d9c882, "$LINEWIDTH");
            externalVarsReference.Add(0x09e85963, "$COLOR2");
            externalVarsReference.Add(0xb4f6068c, "$EMISSIVE_COLOR");
            externalVarsReference.Add(0x9c865576, "$EyeBrightness");
            externalVarsReference.Add(0x626b58e4, "$TRANS");
            externalVarsReference.Add(0x99ed5df3, "gmanEyeGlow");
            externalVarsReference.Add(0xcba2f3ed, "$jawOpen");
            externalVarsReference.Add(0xe1ea5a51, "$ILLUMDEATH");
            externalVarsReference.Add(0xac2455ce, "$EmSpeed");
            externalVarsReference.Add(0x354cd34e, "useglow");
            externalVarsReference.Add(0xc2a33a98, "$IconCoordOffset");
            externalVarsReference.Add(0x64001e52, "$IconCoordScale");
            externalVarsReference.Add(0xfb2f9805, "$CounterIcon");
            externalVarsReference.Add(0x256a1960, "$CounterDigitHundreds");
            externalVarsReference.Add(0x4373b9f9, "$CounterDigitTens");
            externalVarsReference.Add(0x90c26f54, "$CounterDigitOnes");
            externalVarsReference.Add(0xbaeebc0b, "$HealthLights");
            externalVarsReference.Add(0xdea79565, "$FrameNumber1");
            externalVarsReference.Add(0x66a9e338, "$FrameNumber2");
            externalVarsReference.Add(0xcd41b4b8, "$FrameNumber3");
            externalVarsReference.Add(0xe4200216, "origin");
            externalVarsReference.Add(0x9550cca8, "value1");
            externalVarsReference.Add(0x7f787303, "$POSITION");
            externalVarsReference.Add(0xcb9c152d, "advisorMovie");
            externalVarsReference.Add(0xa55cfdd3, "$ANIM");
            externalVarsReference.Add(0xfac4270a, "$LightValue");
            externalVarsReference.Add(0xb5e34aab, "$PercentAwake");
            externalVarsReference.Add(0x435a062f, "$ENERGY");
            externalVarsReference.Add(0xe813cc7e, "$FLOW");
            externalVarsReference.Add(0x38b70d43, "$SCALE");
            externalVarsReference.Add(0x5a8f66c4, "$COLORA");
            externalVarsReference.Add(0x0f09ee7b, "$AnimatePipes");
            externalVarsReference.Add(0xbf319cc2, "$IlluminatePipes");
            externalVarsReference.Add(0x8715f68f, "$PistolChamberReadout");
            externalVarsReference.Add(0x98e238a9, "$PistolClipReadoutOffset");
            externalVarsReference.Add(0x90259463, "$PistolClipReadoutScale");
            externalVarsReference.Add(0xa58adf84, "$PistolHopperReadout");
            externalVarsReference.Add(0xc803a08e, "$InjectedPercent");
            externalVarsReference.Add(0x3666c43e, "$FrameNumber");
            externalVarsReference.Add(0x65f09c96, "$GrenadeLEDBrightness");
            externalVarsReference.Add(0x84e355f4, "$GrenadeLEDFuse");
            externalVarsReference.Add(0xc858079b, "$CableBrightness");
            externalVarsReference.Add(0x507c31f7, "$ChamberBrightness");
            externalVarsReference.Add(0x4f60e501, "$EnergyBallCharged");
            externalVarsReference.Add(0x47940a69, "$ReadyToExplode");
            externalVarsReference.Add(0x3cf1f4a5, "$AmmoColor");
            externalVarsReference.Add(0xefe71421, "$BulletCount");
            externalVarsReference.Add(0x79530848, "$MaxBulletCount");
            externalVarsReference.Add(0x71ee8c47, "$SlideLight");
            externalVarsReference.Add(0xe399c3c7, "$ShotgunHandleLight");
            externalVarsReference.Add(0x5260e007, "$QuickFireLight");
            externalVarsReference.Add(0x72711be3, "$LaserEmitterBrightness");
            externalVarsReference.Add(0x386b35f0, "$LaserEmitterFlowSpeed");
            externalVarsReference.Add(0x73119842, "$LightColor");
        }

    }


}


