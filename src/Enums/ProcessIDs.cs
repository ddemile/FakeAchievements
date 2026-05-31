namespace FakeAchievements.Enums
{
    public class ProcessIDs
    {
        public static ProcessManager.ProcessID AchievementOverlay;
        public static ProcessManager.ProcessID AchievementsMenu;

        public static void RegisterValues()
        {
            AchievementOverlay = new ProcessManager.ProcessID("FakeAchievementOverlay", register: true);
            AchievementsMenu = new ProcessManager.ProcessID("AchievementsMenu", register: true);
        }

        public static void UnregisterValues()
        {
            AchievementOverlay?.Unregister();
            AchievementOverlay = null;
            AchievementsMenu?.Unregister();
            AchievementsMenu = null;
        }
    }
}