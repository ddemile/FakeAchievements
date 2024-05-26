using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeAchievements
{
    public class Achievement
    {
        public string id;
        public Dictionary<string, Dictionary<string, string>> translations;
        public string imagePath;
        public string modId;

        private string GetLocalization(string localization)
        {
            var transformedTranslations = translations.ToDictionary(x => x.Key.ToLower(), x => x.Value);
            string lang = Plugin.RW.options.language.value.ToLower();
            if (transformedTranslations.ContainsKey(lang)) return transformedTranslations[lang][localization];
            return transformedTranslations["english"][localization];
        }

        public string title => GetLocalization("title");
        public string description => GetLocalization("description");
        public string imageName => $"{modId}/achievements/{id}";

        public Achievement(string id, string modId, Dictionary<string, Dictionary<string, string>> localizations)
        {
            this.id = id;
            this.modId = modId;
            this.translations = localizations;

            imagePath = Path.Combine("achievements", this.id, "image.png");
            Futile.atlasManager.UnloadImage(imageName);
            Utils.LoadImage(imageName, imagePath, modId);
        }
    }
}
