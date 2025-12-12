using BepInEx.Logging;
using DevConsole.Commands;
using System;
using System.Globalization;
using UnityEngine;

namespace FakeAchievements
{
    internal static class MyDevConsole
    {
        public static ManualLogSource logSource = BepInEx.Logging.Logger.CreateLogSource("FakeAchievements:ConsoleWrite");

        // Register Commands
        internal static void RegisterCommands()
        {
            new CommandBuilder("achievements")
                .Run(static args =>
                {
                    try
                    {
                        RunCommand(args);
                    }
                    catch (Exception e)
                    {
                        ConsoleWrite("Error in command", Color.red);
                        Plugin.LogError(e);
                    }
                })
                .AutoComplete(static x =>
                {
                    switch (x.Length)
                    {
                        case 0:
                            return ["list", "reload", "grant", "revoke"];
                        case 1:
                            {
                                if (x[0] == "grant")
                                {
                                    return AchievementsManager.achievements.ConvertAll(static achievement => achievement.FullId);
                                }
                                else if (x[0] == "revoke")
                                {
                                    return AchievementsTracker.UnlockedAchievements;
                                }
                                break;
                            }
                        case 2:
                            if (x[0] == "grant")
                            {
                                return ["help-delay: float = 0"];
                            }
                            break;
                        case 3:
                            if (x[0] == "grant")
                            {
                                return ["help-cosmecticOnly: Boolean = False", "True", "False"];
                            }
                            break;
                        default:
                            break;
                    }
                    return [];
                })
                .Help("achievements [subcommand] [args? ...]")
                .Register();
        }

        // Commands handler
        internal static void RunCommand(string[] args)
        {
            if (args.Length == 0)
            {
                ConsoleWrite("Error: No parameter provided", Color.red);
                return;
            }

            string subcommand = args[0].ToLowerInvariant();

            if (args.Length == 1 && (subcommand == "grant" || subcommand == "revoke"))
            {
                ConsoleWrite($"Error: Command requires at least one argument", Color.red);

                string usage = subcommand switch
                {
                    "grant" => "grant [achievementID: string] [delay: float = 0] [cosmeticOnly: bool = False]",
                    "revoke" => "revoke [achievementID: string]",
                    _ => null
                };

                if (!string.IsNullOrEmpty(usage))
                    ConsoleWrite($"Usage: achievements {usage}");
                return;
            }

            switch (subcommand)
            {
                case "list":
                    ConsoleWrite($"Loaded achievements:{Environment.NewLine}{string.Join(Environment.NewLine, AchievementsManager.achievements.ConvertAll(static achievement => $"- {achievement.FullId}{(AchievementsTracker.UnlockedAchievements.Contains(achievement.FullId) ? " [UNLOCKED]" : "")}"))}");
                    break;
                case "reload":
                    AchievementsManager.LoadAchievements();
                    break;
                case "grant":
                    float delay = args.Length > 2 ? float.Parse(args[2].ToLowerInvariant().Replace("f", ""), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) : 0f;
                    bool cosmecticOnly = args.Length > 3 && args[3].ToLowerInvariant() == "true";

                    AchievementsManager.GrantAchievement(args[1], delay, cosmecticOnly);
                    break;
                case "revoke":
                    AchievementsManager.RevokeAchievement(args[1]);
                    break;
                default:
                    ConsoleWrite($"Unknown command: {subcommand}", Color.red);
                    break;
            }
        }

        public static void ConsoleWrite(string message, Color color)
        {
            try
            {
                GameConsoleWriteLine(message, color);
            }
            catch { }
        }

        public static void ConsoleWrite(string message)
        {
            try
            {
                GameConsoleWriteLine(message, Color.white);
            }
            catch { }
        }

        private static void GameConsoleWriteLine(string message, Color color)
        {
            DevConsole.GameConsole.WriteLine(message, color);
            logSource.LogMessage(message);
        }
    }
}