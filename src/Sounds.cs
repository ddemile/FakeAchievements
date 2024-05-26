using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeAchievements
{
    public static class Sounds
    {
        public static SoundID STEAM_ACHIEVEMENT { get; private set; }

        // Call Initialize() from your plugin's Awake method.
        internal static void Initialize()
        {
            STEAM_ACHIEVEMENT = new SoundID("Steam_Achievement", true);
        }
    }
}
