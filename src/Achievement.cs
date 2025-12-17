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
        public string ImageName => $"{ModId}/achievements/{Id}";

        public Achievement(string id, string modId, Dictionary<string, Dictionary<string, string>> localizations)
        {
            Id = id;
            ModId = modId;
            translations = localizations.ToDictionary(static x => x.Key.ToLower(), static x => x.Value);

            imagePath = Path.Combine("achievements", Id, "image.png");
            Futile.atlasManager.UnloadImage(ImageName);
            Utils.LoadImage(ImageName, imagePath, modId);
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