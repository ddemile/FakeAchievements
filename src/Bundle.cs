using RWCustom;
using UnityEngine;

namespace FakeAchievements
{
    internal static class Bundle
    {
        private static AssetBundle bundle;

        internal static void Load()
        {
            bundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("assetsbundles/fakeachievements"));
            var shader = bundle.LoadAsset<Shader>("assets/fakeachievements/fagreyscale.shader");
            Custom.rainWorld.Shaders.Add("FAGreyscale", FShader.CreateShader(shader.name, shader));
        }
    }
}
