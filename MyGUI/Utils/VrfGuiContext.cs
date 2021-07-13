using MyGUI.Controls;
using MyGUI.Types.Renderer;
using SteamDatabase.ValvePak;
using MyValveResourceFormat;

namespace MyGUI.Utils
{
    public class VrfGuiContext
    {
        public string FileName { get; }

        public Package CurrentPackage { get; }

        public MaterialLoader MaterialLoader { get; }

        public ShaderLoader ShaderLoader { get; }
        public GPUMeshBufferCache MeshBufferCache { get; }
        public AdvancedGuiFileLoader FileLoader { get; }
        public AdvancedGuiFileLoader ParentFileLoader { get; }

        public QuadIndexBuffer QuadIndices
        {
            get
            {
                if (quadIndices == null)
                {
                    quadIndices = new QuadIndexBuffer(65532);
                }

                return quadIndices;
            }
        }

        private QuadIndexBuffer quadIndices;

        public VrfGuiContext(string fileName, TreeViewWithSearchResults.TreeViewPackageTag package)
        {
            FileName = fileName;
            CurrentPackage = package?.Package;
            ParentFileLoader = package?.ParentFileLoader;
            MaterialLoader = new MaterialLoader(this);
            ShaderLoader = new ShaderLoader();
            FileLoader = new AdvancedGuiFileLoader(this);
            MeshBufferCache = new GPUMeshBufferCache();
        }

        public Resource LoadFileByAnyMeansNecessary(string file) =>
            FileLoader.LoadFile(file);

        public void ClearCache() => FileLoader.ClearCache();
    }
}
