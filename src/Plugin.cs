using System;
using BepInEx;
using UnityEngine;

namespace FakeAchievements
{
    [BepInPlugin(MOD_ID, "Fake Achievements", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "ddemile.fake_achievements";

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
        }
        
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }
    }
}