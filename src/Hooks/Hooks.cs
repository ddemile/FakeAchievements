using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            FakeAchievementManager.instance?.GrafUpdate(timeStacker);
        }
    }
}
