using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FakeAchievements
{
    public class Achievement
    {
        private readonly Dictionary<string, Dictionary<string, string>> translations;
        private readonly string imagePath;

        public string Id { get; }
        public string ModId { get; }

        public string FullId => $"{ModId}/{Id}";

        public string Title => GetLocalization("title");
        public string Description => GetLocalization("description");
        public bool Achieved => AchievementsTracker.UnlockedAchievements.Contains(FullId);
        public string UnlockedImageName => $"{ModId}/achievements/{Id}";
        public string LockedImageName => $"{UnlockedImageName}/locked";
        public string ImageName => (HasLockedImage && !Achieved) ? LockedImageName : UnlockedImageName;
        
        public bool Hidden { get; }
        public bool VisibleInMenu { get; }

        public bool HasLockedImage { get; }

        public Achievement(string id, string modId, Dictionary<string, Dictionary<string, string>> localizations, bool hidden = false, bool visibleInMenu = true, bool hasLockedImage = false)
        {
            Id = id;
            ModId = modId;
            translations = localizations.ToDictionary(static x => x.Key.ToLower(), static x => x.Value);
            Hidden = hidden;
            VisibleInMenu = visibleInMenu;
            HasLockedImage = hasLockedImage;

            imagePath = Path.Combine("achievements", Id, "image.png");
            Futile.atlasManager.UnloadImage(UnlockedImageName);
            Utils.LoadImage(UnlockedImageName, imagePath, modId);

            Futile.atlasManager.UnloadImage(LockedImageName);
            if (hasLockedImage)
            {
                string lockedImagePath = Path.Combine("achievements", Id, "image_locked.png");

                Utils.LoadImage(LockedImageName, lockedImagePath, modId);
            }
        }

        private string GetLocalization(string localization)
        {
            string lang = Plugin.RW.options.language.value.ToLower();

            return translations.TryGetValue(lang, out Dictionary<string, string> translation)
                ? translation[localization]
                : translations["english"][localization];
        }
    }
}