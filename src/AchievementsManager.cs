using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace FakeAchievements
{
    public class AchievementsManager
    {
        public static List<Achievement> achievements;

        public static void LoadAchievements()
        {
            Plugin.Log("Loading achievements");

            achievements = [];

            foreach (ModManager.Mod mod in ModManager.ActiveMods)
            {
                string achievementsPath = Path.Combine(mod.path, "achievements");

                if (!Directory.Exists(achievementsPath)) continue;

                string[] directories = Directory.GetDirectories(achievementsPath);

                foreach (string directory in directories)
                {
                    string achievementId = new DirectoryInfo(directory).Name;
                    string achievementPath = Path.Combine(achievementsPath, achievementId);

                    Plugin.Log("Found achievement: " + achievementId + " | " + achievementPath.Replace(mod.path, mod.name));

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
            return achievements.Find(achievement => achievement.Id == achievementResolvable || achievement.FullId == achievementResolvable);
        }

        [Obsolete("This method was replaced by the new GrantAchievement method")]
        public static void ShowAchievement(string achievementResolvable)
        {
            GrantAchievement(achievementResolvable, 0f, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GrantAchievement(string achievementResolvable, bool cosmeticOnly = false)
        {
            GrantAchievement(achievementResolvable, 0f, cosmeticOnly);
        }

        public static void GrantAchievement(string achievementResolvable, float delay, bool cosmeticOnly = false)
        {
            Achievement achievement = ResolveAchievement(achievementResolvable) ?? throw new ArgumentException($"Achievement not found: {achievementResolvable}", nameof(achievementResolvable));

            if (cosmeticOnly || AchievementsTracker.UnlockAchievement(achievement.FullId))
            {
                Plugin.Log($"Displaying achievement: {achievement.FullId}");

                AchievementMenu.RequestMenu(achievement, delay);
            }
        }

        public static void RevokeAchievement(string achievementResolvable)
        {
            Achievement achievement = ResolveAchievement(achievementResolvable);

            string achievementId = achievement != null ? achievement.FullId : achievementResolvable;

            if (!AchievementsTracker.LockAchievement(achievementId))
            {
                throw new InvalidOperationException($"Couldn't revoke achievement: {achievementId}");
            }
        }

        [Obsolete("This field was replaced by AchievementMenu's new activeInstance and waitingInstances fields.")]
        public static List<AchievementMenu> menuInstances = [];
    }
}