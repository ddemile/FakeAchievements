namespace FakeAchievements.Hooks
{
    internal static class Hooks
    {
        public static void Register()
        {
            On.MainLoopProcess.GrafUpdate += MainLoopProcess_GrafUpdate;
            On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
        }

        private static void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
        {
            if (self.pendingProcess == null && ID == Enums.ProcessIDs.AchievementsMenu)
            {
                self.currentMainLoop = new AchievementsMenu(self);
            }
            orig(self, ID);
        }

        private static void MainLoopProcess_GrafUpdate(On.MainLoopProcess.orig_GrafUpdate orig, MainLoopProcess self, float timeStacker)
        {
            orig(self, timeStacker);

            AchievementOverlay.UpdateInstances(timeStacker);
        }
    }
}