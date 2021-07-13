using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyGUI.Utils;
using MyValveResourceFormat;
using MyValveResourceFormat.ResourceTypes;
using MyValveResourceFormat.ResourceTypes.ModelAnimation;
using MyValveResourceFormat.Serialization;

namespace MyGUI.Types.Renderer
{
    internal static class AnimationGroupLoader
    {
        public static IEnumerable<Animation> LoadAnimationGroup(Resource resource, VrfGuiContext vrfGuiContext)
        {
            var data = resource.DataBlock.AsKeyValueCollection();

            // Get the list of animation files
            var animArray = data.GetArray<string>("m_localHAnimArray").Where(a => a != null);
            // Get the key to decode the animations
            var decodeKey = data.GetSubCollection("m_decodeKey");

            var animationList = new List<Animation>();

            // Load animation files
            foreach (var animationFile in animArray)
            {
                animationList.AddRange(LoadAnimationFile(animationFile, decodeKey, vrfGuiContext));
            }

            return animationList;
        }

        public static IEnumerable<Animation> TryLoadSingleAnimationFileFromGroup(Resource resource, string animationName, VrfGuiContext vrfGuiContext)
        {
            var data = resource.DataBlock.AsKeyValueCollection();

            // Get the list of animation files
            var animArray = data.GetArray<string>("m_localHAnimArray").Where(a => a != null);
            // Get the key to decode the animations
            var decodeKey = data.GetSubCollection("m_decodeKey");

            var animation = animArray.FirstOrDefault(a => a != null && a.EndsWith($"{animationName}.vanim"));

            if (animation != default)
            {
                return LoadAnimationFile(animation, decodeKey, vrfGuiContext);
            }
            else
            {
                return null;
            }
        }

        private static IEnumerable<Animation> LoadAnimationFile(string animationFile, IKeyValueCollection decodeKey, VrfGuiContext vrfGuiContext)
        {
            var animResource = vrfGuiContext.LoadFileByAnyMeansNecessary(animationFile + "_c");

            if (animResource == null)
            {
                return null;
            }

            // Build animation classes
            return Animation.FromResource(animResource, decodeKey);
        }
    }
}
