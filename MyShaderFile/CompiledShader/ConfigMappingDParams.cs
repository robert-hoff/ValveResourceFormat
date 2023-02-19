using System;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace ValveResourceFormat.CompiledShader
{
    public class ConfigMappingDParams
    {
        private ShaderFile shaderfile;

        public ConfigMappingDParams(ShaderFile shaderfile)
        {
            this.shaderfile = shaderfile;
            GenerateOffsetAndStateLookups(shaderfile);
        }

        int[] offsets;
        int[] nr_states;

        private void GenerateOffsetAndStateLookups(ShaderFile shaderFile)
        {
            if (shaderFile.DBlocks.Count == 0)
            {
                offsets = Array.Empty<int>();
                nr_states = Array.Empty<int>();
                return;
            }

            offsets = new int[shaderFile.DBlocks.Count];
            nr_states = new int[shaderFile.DBlocks.Count];

            offsets[0] = 1;
            nr_states[0] = shaderFile.DBlocks[0].Arg2 + 1;

            for (int i = 1; i < shaderFile.DBlocks.Count; i++)
            {
                nr_states[i] = shaderFile.DBlocks[i].Arg2 + 1;
                offsets[i] = offsets[i - 1] * nr_states[i - 1];
            }
        }

        public int[] GetConfigState(long zframeId)
        {
            int[] state = new int[nr_states.Length];
            for (int i = 0; i < nr_states.Length; i++)
            {
                state[i] = (int)(zframeId / offsets[i]) % (nr_states[i]);
            }
            return state;
        }

        public void ShowOffsetAndLayersArrays(bool hex = true)
        {
            ShowIntArray(offsets, 8, "offsets", hex: hex);
            ShowIntArray(nr_states, 8, "layers");
        }

    }
}
