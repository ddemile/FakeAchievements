using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace FakeAchievements
{
    /// <summary>
    /// A simple tracker of already unlocked custom achievements.
    /// </summary>
    public static class AchievementsTracker
    {
        /// <summary>
        /// A list of already unlocked achievements; By default, these are not triggered again unless manually locked or bypassed.
        /// </summary>
        private static readonly List<string> _unlockedAchievements = [];

        /// <summary>
        /// The path to the file where unlocked achievement IDs are stored.
        /// </summary>
        private static readonly string PathToUnlocksFile;

        /// <inheritdoc cref="_unlockedAchievements"/>
        public static readonly ReadOnlyCollection<string> UnlockedAchievements;

        static AchievementsTracker()
        {
            UnlockedAchievements = new(_unlockedAchievements);

            // Not sure if other folders are saved to cloud by Steam, but ModConfigs has a near-guarantee of being stored, which is nice for something persistent like achievements.
            string folderPath = Path.Combine(Application.persistentDataPath, "ModConfigs", "FakeAchievements");

            if (!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                }
                catch (Exception ex)
                {
                    Plugin.LogError("Failed to create FakeAchievements directory!");
                    Plugin.LogError(ex);
                }
            }

            PathToUnlocksFile = Path.Combine(folderPath, "UnlockedAchievements.txt");
        }

        /// <summary>
        /// Determines if the given achievement can be unlocked by the player.
        /// </summary>
        /// <param name="achievementID">The ID of the achievement to evaluate.</param>
        /// <returns><c>true</c> if the achievement can be unlocked, <c>false</c> otherwise.</returns>
        public static bool CanUnlockAchievement(string achievementID)
        {
            return !_unlockedAchievements.Contains(achievementID); // Add any other logic or requirements for unlocking achievements here
        }

        /// <summary>
        /// Sets a given achievement ID as "locked", allowing it to be triggered by the player again.
        /// </summary>
        /// <param name="achievementID">The ID of the achievement to "lock".</param>
        /// <returns>
        ///     <c>true</c> if the achievement has been successfully locked, <c>false</c> otherwise.
        ///     This method returns <c>false</c> if <paramref name="achievementID"/> was not found in the list of unlocked achievements.
        /// </returns>
        public static bool LockAchievement(string achievementID)
        {
            if (string.IsNullOrEmpty(achievementID))
                throw new ArgumentException("Achievement ID cannot be empty or null.", nameof(achievementID));

            if (!_unlockedAchievements.Contains(achievementID)) return false;

            Plugin.Log($"Locking achievement: {achievementID}");

            return _unlockedAchievements.Remove(achievementID);
        }

        /// <summary>
        /// Sets a given achievement ID as "unlocked", preventing it from being triggered again in the future.
        /// </summary>
        /// <param name="achievementID">The ID of the achievement to "unlock".</param>
        /// <param name="forceUnlock">If true, the achievement will be "unlocked" even if it was already unlocked before.</param>
        /// <returns><c>true</c> if the achievement has been successfully unlocked, <c>false</c> otherwise.</returns>
        public static bool UnlockAchievement(string achievementID, bool forceUnlock = false)
        {
            if (string.IsNullOrEmpty(achievementID))
                throw new ArgumentException("Achievement ID cannot be empty or null.", nameof(achievementID));

            if (!forceUnlock && !CanUnlockAchievement(achievementID)) return false;

            Plugin.Log($"Unlocking achievement: {achievementID}");

            if (!_unlockedAchievements.Contains(achievementID))
                _unlockedAchievements.Add(achievementID);

            return true;
        }

        /// <summary>
        /// Loads the stored list of unlocked achievement IDs from the mod's configured path.
        /// </summary>
        internal static void LoadUnlockedAchievements()
        {
            if (!File.Exists(PathToUnlocksFile)) return;

            try
            {
                _unlockedAchievements.Clear();

                string[] rawData = File.ReadAllText(PathToUnlocksFile).Split(';');

                // As of right now, the version can be ignored. What we actually need is the list of IDs that come afterwards.

                if (rawData.Length < 2)
                    throw new InvalidDataException("Save file is corrupted; Cannot retrieve unlocked achievements.");

                _unlockedAchievements.AddRange(rawData[1].Split([','], StringSplitOptions.RemoveEmptyEntries));

                Plugin.Log($"Loaded {_unlockedAchievements.Count} unlocked achievement(s).");
            }
            catch (Exception ex)
            {
                Plugin.LogError("Failed to load unlocked achievements data!");
                Plugin.LogError(ex);
            }
        }

        /// <summary>
        /// Saves the list of unlocked achievements to the configured file of this mod.
        /// </summary>
        internal static void SaveUnlockedAchievements()
        {
            string rawData = $"v{Plugin.MOD_VERSION};{string.Join(",", _unlockedAchievements)}"; // Version is stored for future-proofing, in case the API changes and we need to migrate the old file format

            try
            {
                File.WriteAllText(PathToUnlocksFile, rawData);

                Plugin.Log($"Saved {_unlockedAchievements.Count} unlocked achievement(s).");
            }
            catch (Exception ex)
            {
                Plugin.LogError("Failed to save unlocked achievements data!");
                Plugin.LogError(ex);
            }
        }
    }
}