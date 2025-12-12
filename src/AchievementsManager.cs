using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FakeAchievements
{
    public class AchievementsManager
    {
        public const float speedFactor = 1.5f;

        public static List<Achievement> achievements;

        public static void LoadAchievements()
        {
            Plugin.Log("Loading achievements");

            List<ModManager.Mod> mods = (from mod in ModManager.InstalledMods where mod.enabled select mod).ToList();

            achievements = [];

            foreach (ModManager.Mod mod in mods)
            {
                string achievementsPath = Path.Combine(mod.path, "achievements");
                if (!Directory.Exists(achievementsPath)) continue;

                string[] directories = Directory.GetDirectories(achievementsPath);

                foreach (string directory in directories)
                {
                    string achievementId = new DirectoryInfo(directory).Name;
                    string achievementPath = Path.Combine(achievementsPath, achievementId);

                    Plugin.Log("Found achievement: " + achievementId + " | " + achievementPath);

                    string oldLangsFile = Path.Combine(achievementPath, "info.json");

                    string langsFile = File.Exists(oldLangsFile) ? oldLangsFile : Path.Combine(achievementPath, "langs.json");
                    var localizations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(langsFile));

                    Achievement achievement = new Achievement(achievementId, mod.id, localizations);

                    achievements.Add(achievement);
                }
            }
        }

        public static Achievement ResolveAchievement(string achievementResolvable)
        {
            return achievements.Find(achievement => $"{achievement.modId}/{achievement.id}" == achievementResolvable || achievement.id == achievementResolvable);
        }

        [Obsolete("This method was replaced by the new GrantAchievement method")]
        public static void ShowAchievement(string achievementResolvable)
        {
            GrantAchievement(achievementResolvable, true);
        }

        public static void GrantAchievement(string achievementResolvable, bool cosmeticOnly = false)
        {
            Achievement achievement = ResolveAchievement(achievementResolvable) ?? throw new Exception($"Achievement not found: {achievementResolvable}");

            string achievementId = $"{achievement.modId}/{achievement.id}";

            if (cosmeticOnly || AchievementsTracker.UnlockAchievement(achievementId))
            {
                Plugin.Log($"Displaying achievement: {achievementId}");

                menuInstances.Add(
                    new AchievementMenu(Plugin.RW.processManager, achievement)
                );
            }
        }

        public static void RevokeAchievement(string achievementResolvable)
        {
            Achievement achievement = ResolveAchievement(achievementResolvable);

            string achievementId = achievement != null ? $"{achievement.modId}/{achievement.id}" : achievementResolvable;

            if (!AchievementsTracker.LockAchievement(achievementId))
            {
                throw new Exception($"Couldn't revoke achievement: {achievementId}");
            }
        }

        public static List<AchievementMenu> menuInstances = [];
    }
}
