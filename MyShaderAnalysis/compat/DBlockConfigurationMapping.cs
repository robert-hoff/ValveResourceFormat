using MyShaderAnalysis.vcsparsing;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;


namespace MyShaderAnalysis.compat
{


    public class DBlockConfigurationMapping
    {


        ShaderFile shaderfile;

        public DBlockConfigurationMapping(ShaderFile shaderfile)
        {
            this.shaderfile = shaderfile;
            GenerateOffsetAndLayers(shaderfile);
        }



        static int[] offsets;
        static int[] layers;

        private void GenerateOffsetAndLayers(ShaderFile shaderFile)
        {

            if (shaderFile.dBlocks.Count == 0)
            {
                offsets = new int[] { 1 };
                layers = new int[] { 0 };
                return;
            }

            offsets = new int[shaderFile.dBlocks.Count];
            layers = new int[shaderFile.dBlocks.Count];

            offsets[0] = 1;
            layers[0] = shaderFile.dBlocks[0].arg2;

            for (int i = 1; i < shaderFile.dBlocks.Count; i++)
            {
                int curLayer = shaderFile.dBlocks[i].arg2;
                layers[i] = curLayer;
                offsets[i] = offsets[i - 1] * (layers[i - 1] + 1);
            }
        }


        public int[] GetConfigState(long zframeId)
        {
            int[] state = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                long res = (zframeId / offsets[i]) % (layers[i] + 1);
                state[i] = (int)res;
            }
            return state;
        }


        public void ShowOffsetAndLayersArrays(bool hex = true)
        {
            ShowIntArray(offsets, 8, "offsets", hex: hex);
            ShowIntArray(layers, 8, "layers");

        }





    }
}








