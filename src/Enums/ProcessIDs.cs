namespace FakeAchievements.Enums
{
    public class ProcessIDs
    {
        public static ProcessManager.ProcessID FakeAchievementMenu;

        public static void RegisterValues()
        {
            FakeAchievementMenu = new ProcessManager.ProcessID("FakeAchievementMenu", register: true);
        }

        public static void UnregisterValues()
        {
            FakeAchievementMenu?.Unregister();
            FakeAchievementMenu = null;
        }
    }
}