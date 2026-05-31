using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeAchievements
{
    internal class SteamFetcher
    {
        internal static List<SteamAchievement> FetchAchievements()
        {
            var list = new List<SteamAchievement>();

            uint count = SteamUserStats.GetNumAchievements();

            for (uint i = 0; i < count; i++)
            {
                string apiName = SteamUserStats.GetAchievementName(i);

                if (string.IsNullOrEmpty(apiName))
                    continue;

                SteamUserStats.GetAchievement(apiName, out bool achieved);

                string displayName = SteamUserStats.GetAchievementDisplayAttribute(
                    apiName, "name");

                string description = SteamUserStats.GetAchievementDisplayAttribute(
                    apiName, "desc");

                bool hidden = SteamUserStats.GetAchievementDisplayAttribute(
                    apiName, "hidden") == "1";

                int icon = SteamUserStats.GetAchievementIcon(apiName);

                list.Add(new SteamAchievement
                {
                    ApiName = apiName,
                    DisplayName = displayName,
                    Description = description,
                    Icon = icon,
                    Achieved = achieved,
                    Hidden = hidden
                });
            }

            return list;
        }        
    }

    internal struct SteamAchievement
    {
        public string ApiName;
        public string DisplayName;
        public string Description;
        public int Icon;
        public bool Achieved;
        public bool Hidden;
    }
}
