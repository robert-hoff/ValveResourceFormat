using System;
using System.Numerics;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.IO;
using ValveResourceFormat.Serialization;

namespace ValveResourceFormat.ResourceTypes
{
    public class Mesh : KeyValuesOrNTRO
    {
        public VBIB VBIB
        {
            get
            {
                if (cachedVBIB == null)
                {
                    //new format has VBIB block, for old format we can get it from NTRO DATA block
                    cachedVBIB = Resource.VBIB ?? new VBIB(Data);
                }
                return cachedVBIB;
            }
            set
            {
                cachedVBIB = value;
            }
        }

        public Vector3 MinBounds { get; private set; }
        public Vector3 MaxBounds { get; private set; }

        public Morph MorphData { get; set; }

        private VBIB cachedVBIB { get; set; }

        public Mesh(BlockType type) : base(type, "PermRenderMeshData_t")
        {
        }

        public void GetBounds()
        {
            var sceneObjects = Data.GetArray("m_sceneObjects");
            if (sceneObjects.Length == 0)
            {
                MinBounds = MaxBounds = new Vector3(0, 0, 0);
                return;
            }

            var minBounds = sceneObjects[0].GetSubCollection("m_vMinBounds").ToVector3();
            var maxBounds = sceneObjects[0].GetSubCollection("m_vMaxBounds").ToVector3();

            for (var i = 1; i < sceneObjects.Length; ++i)
            {
                var localMin = sceneObjects[i].GetSubCollection("m_vMinBounds").ToVector3();
                var localMax = sceneObjects[i].GetSubCollection("m_vMaxBounds").ToVector3();

                minBounds.X = Math.Min(minBounds.X, localMin.X);
                minBounds.Y = Math.Min(minBounds.Y, localMin.Y);
                minBounds.Z = Math.Min(minBounds.Z, localMin.Z);
                maxBounds.X = Math.Max(maxBounds.X, localMax.X);
                maxBounds.Y = Math.Max(maxBounds.Y, localMax.Y);
                maxBounds.Z = Math.Max(maxBounds.Z, localMax.Z);
            }

            MinBounds = minBounds;
            MaxBounds = maxBounds;
        }

        public static bool IsCompressedNormalTangent(IKeyValueCollection drawCall)
        {
            if (drawCall.ContainsKey("m_bUseCompressedNormalTangent"))
            {
                return drawCall.GetProperty<bool>("m_bUseCompressedNormalTangent");
            }

            if (!drawCall.ContainsKey("m_nFlags"))
            {
                return false;
            }

            var flags = drawCall.GetProperty<object>("m_nFlags");

            return flags switch
            {
                string flagsString => flagsString.Contains("MESH_DRAW_FLAGS_USE_COMPRESSED_NORMAL_TANGENT", StringComparison.InvariantCulture),
                long flagsLong => ((RenderMeshDrawPrimitiveFlags)flagsLong & RenderMeshDrawPrimitiveFlags.UseCompressedNormalTangent) != 0,
                byte flagsByte => ((RenderMeshDrawPrimitiveFlags)flagsByte & RenderMeshDrawPrimitiveFlags.UseCompressedNormalTangent) != 0,
                _ => false
            };
        }

        public void LoadExternalMorphData(IFileLoader fileLoader)
        {
            if (MorphData == null)
            {
                var morphSetPath = Data.GetStringProperty("m_morphSet");
                if (!string.IsNullOrEmpty(morphSetPath))
                {
                    var morphSetResource = fileLoader.LoadFile(morphSetPath + "_c");
                    if (morphSetResource != null)
                    {
                        MorphData = morphSetResource.GetBlockByType(BlockType.MRPH) as Morph;
                    }
                }
            }

            MorphData?.LoadFlexData(fileLoader);
        }
    }
}
