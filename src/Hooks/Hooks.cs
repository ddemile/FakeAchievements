using System;
using Menu;
using MonoMod.Cil;
using UnityEngine;

namespace FakeAchievements.Hooks
{
    internal static class Hooks
    {
        public static void Register()
        {
            On.MainLoopProcess.GrafUpdate += MainLoopProcess_GrafUpdate;
            On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
            On.Menu.MainMenu.ctor += MainMenu_ctor;
            IL.Menu.MainMenu.AddMainMenuButton += MainMenu_AddMainMenuButton;
        }

        private static void MainMenu_AddMainMenuButton(ILContext il)
        {
            try
            {
                ILCursor c = new(il);
                c.GotoNext(MoveType.After, x => x.MatchLdcI4(8));
                c.MoveAfterLabels();
                c.EmitDelegate<Func<int, int>>((_) => 12);
            } catch (Exception ex)
            {
                Plugin.LogError($"Failed to hook MainMenu.AddMainMenuButton: {ex}");
            }
        }

        private static void MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig(self, manager, showRegionSpecificBkg);

			float buttonWidth = MainMenu.GetButtonWidth(self.CurrLang);
            Vector2 pos = new Vector2(0f, 0f);
			Vector2 size = new Vector2(buttonWidth, 30f);
            foreach (SimpleButton button in self.mainMenuButtons)
            {
                if (button.signalText == "COLLECTION")
                {
                    pos = button.pos;
                    size = button.size;
                    break;
                }
            }
			self.AddMainMenuButton(new SimpleButton(self, self.pages[0], "ACHIEVEMENTS", "ACHIEVEMENTS", pos, size), () => AchievementsButtonPressed(self), 2);
        }

        private static void AchievementsButtonPressed(MainMenu menu)
        {
            menu.manager.RequestMainProcessSwitch(Enums.ProcessIDs.AchievementsMenu);
            menu.PlaySound(SoundID.MENU_Switch_Page_In);
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