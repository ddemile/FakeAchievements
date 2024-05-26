using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace FakeAchievements
{
    public static class Utils
    {
        public static void WordWrapLabel(FLabel label, float maxWidth, int maxLines)
        {
            string text = "";
            string[] array = label.text.Split(Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string text2 = "";
                if (array[i].Length != 0)
                {
                    string[] array2 = array[i].Split(new char[] { ' ' });
                    if (array2.Length > 1)
                    {
                        for (int j = 0; j < array2.Length; j++)
                        {
                            text2 = text2 + array2[j] + " ";
                            label.text = text2;
                            if (label.textRect.width > maxWidth)
                            {
                                if (text.Split(Environment.NewLine.ToCharArray()).Length >= maxLines)
                                {
                                    label.text = text.Remove(text.Length - 1, 1) + "...";
                                    return;
                                }
                                text = text + Environment.NewLine + array2[j] + " ";
                                text2 = array2[j] + " ";
                            }
                            else
                            {
                                text = text + array2[j] + " ";
                            }
                        }
                        if (i != array.Length - 1)
                        {
                            text += Environment.NewLine;
                        }
                    }
                    else
                    {
                        for (int k = 0; k < array[i].Length; k++)
                        {
                            text2 += array[i][k].ToString();
                            label.text = text2;
                            if (label.textRect.width > maxWidth)
                            {
                                if (text.Split(Environment.NewLine.ToCharArray()).Length >= maxLines)
                                {
                                    label.text = text.Remove(text.Length - 1, 1) + "...";
                                    return;
                                }
                                text = text + Environment.NewLine + array[i][k].ToString();
                                text2 = array[i][k].ToString();
                            }
                            else
                            {
                                text += array[i][k].ToString();
                            }
                        }
                    }
                }
            }
            label.text = text;
        }

        public static FAtlas LoadImage(string name, string imagePath, string modId)
        {
            FAtlasManager atlasManager = Futile.atlasManager;
            if (atlasManager.DoesContainAtlas(name))
            {
                return atlasManager.GetAtlasWithName(name);
            }

            string filePath = ResolveFilePath(imagePath, modId);

            Texture2D texture = LoadTexture(filePath);

            Plugin.Log("Creating achievement atlas");

            FAtlas atlas = new FAtlas(name, texture, FAtlasManager._nextAtlasIndex++, false); new FAtlas(name, texture, FAtlasManager._nextAtlasIndex++, false);

            Plugin.Log("Adding achievement atlas");

            atlasManager.AddAtlas(atlas);

            Plugin.Log("Added achievement atlas");

            return atlas;
        }

        public static string ResolveFilePath(string path, string modId)
        {
            ModManager.Mod mod = ResolveMod(modId);

            if (mod.hasTargetedVersionFolder)
            {
                string text2 = Path.Combine(mod.TargetedPath, path.ToLowerInvariant());
                if (File.Exists(text2))
                {
                    return text2;
                }
            }
            if (mod.hasNewestFolder)
            {
                string text3 = Path.Combine(mod.NewestPath, path.ToLowerInvariant());
                if (File.Exists(text3))
                {
                    return text3;
                }
            }
            string text4 = Path.Combine(mod.path, path.ToLowerInvariant());
            if (File.Exists(text4))
            {
                return text4;
            }

            return Path.Combine(Custom.RootFolderDirectory(), path.ToLowerInvariant());
        }

        public static ModManager.Mod ResolveMod(string modId)
        {
            return (from mod in ModManager.InstalledMods where mod.enabled select mod).ToList().Find(mod => mod.id == modId);
        }

        public static Texture2D LoadTexture(string path)
        {
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            return AssetManager.SafeWWWLoadTexture(ref texture2D, path, false, true);
        }
    }
}
