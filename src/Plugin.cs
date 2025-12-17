global using static FakeAchievements.MyDevConsole;
using BepInEx;
using BepInEx.Logging;
using FakeAchievements.Enums;

namespace FakeAchievements
{
    [BepInPlugin(MOD_ID, "Fake Achievements", MOD_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal const string MOD_ID = "ddemile.fake_achievements";
        internal const string MOD_VERSION = "0.1.4";

        public static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("FakeAchievements");

        public static RainWorld RW => FindObjectOfType<RainWorld>();

        public static void Log(object msg)
        {
            log.LogInfo(msg);
        }

        public static void LogError(object msg)
        {
            log.LogError(msg);
        }

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.RainWorld.PostModsInit += OnPostModsInit;

            ProcessIDs.RegisterValues();
            SoundIDs.RegisterValues();

            // Put your custom hooks here!
            Hooks.Hooks.Register();
        }

        public void OnDisable()
        {
            AchievementsTracker.SaveUnlockedAchievements();

            ProcessIDs.UnregisterValues();
            SoundIDs.UnregisterValues();
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Futile.atlasManager.LoadImage("illustrations/achievement_background");

            AchievementsManager.LoadAchievements();

            AchievementsTracker.LoadUnlockedAchievements();
        }

        private void OnPostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            try { RegisterCommands(); } catch { }
        }
    }
}