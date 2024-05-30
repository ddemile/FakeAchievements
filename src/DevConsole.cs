﻿using BepInEx.Logging;
using DevConsole.Commands;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FakeAchievements
{
    internal static class MyDevConsole
    {
        public static ManualLogSource logSource = BepInEx.Logging.Logger.CreateLogSource("SilkSlug:ConsoleWrite");
        public static RainWorld RW => UnityEngine.Object.FindObjectOfType<RainWorld>();

        // Register Commands
        internal static void RegisterCommands()
        {
            new CommandBuilder("achievements")
                .Run(args =>
                {
                    try
                    {
                        RunCommand(args);
                    }
                    catch (Exception e) { ConsoleWrite("Error in command", Color.red); Plugin.LogError(e); }
                })
                .AutoComplete(x =>
                {
                    if (x.Length == 0)
                    {
                        return new string[] { "reload", "grant" };
                    }
                    if (x.Length == 1)
                    {
                        if (x[0] == "grant")
                        {
                            return AchievementsManager.achievements.ConvertAll(achievement => $"{achievement.modId}/{achievement.id}");
                        } 
                    }
                    return new string[0];
                })
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

            if (args[0] == "reload")
            {
                AchievementsManager.LoadAchievements();
            } else if (args[0] == "grant")
            {
                AchievementsManager.ShowAchievement(args[1]);
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
        public static void ConsoleWrite(string message = "")
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
