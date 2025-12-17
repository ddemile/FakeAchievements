namespace FakeAchievements.Enums
{
    public static class SoundIDs
    {
        public static SoundID STEAM_ACHIEVEMENT { get; private set; }

        public static void RegisterValues()
        {
            STEAM_ACHIEVEMENT = new SoundID("Steam_Achievement", register: true);
        }

        public static void UnregisterValues()
        {
            STEAM_ACHIEVEMENT?.Unregister();
            STEAM_ACHIEVEMENT = null;
        }
    }
}
