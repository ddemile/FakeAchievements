global using static FakeAchievements.MyDevConsole;
using BepInEx;
using BepInEx.Logging;

namespace FakeAchievements
{
    [BepInPlugin(MOD_ID, "Fake Achievements", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "ddemile.fake_achievements";

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
            try { RegisterCommands(); } catch { }

            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            Hooks.Hooks.Register();
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Futile.atlasManager.LoadImage("illustrations/achievement_background");

            Sounds.Initialize();
            FakeAchievementManager.LoadAchievements();
        }
    }
}