namespace FakeAchievements.Hooks
{
    internal static class Hooks
    {
        public static void Register()
        {
            On.MainLoopProcess.GrafUpdate += MainLoopProcess_GrafUpdate;
        }

        private static void MainLoopProcess_GrafUpdate(On.MainLoopProcess.orig_GrafUpdate orig, MainLoopProcess self, float timeStacker)
        {
            orig(self, timeStacker);

            AchievementMenu.UpdateInstances(timeStacker);
        }
    }
}