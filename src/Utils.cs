using Microsoft.Win32;
using RWCustom;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Steamworks;

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
                    string[] array2 = array[i].Split([' ']);
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

            FAtlas atlas = new FAtlas(name, texture, FAtlasManager._nextAtlasIndex++, false);

            atlasManager.AddAtlas(atlas);

            return atlas;
        }

        public static FAtlas LoadSteamImage(string name, int imageHandle)
        {
            FAtlasManager atlasManager = Futile.atlasManager;
            if (atlasManager.DoesContainAtlas(name))
            {
                return atlasManager.GetAtlasWithName(name);
            }

            Texture2D texture = SteamImageToTexture2D(imageHandle);

            FAtlas atlas = new FAtlas(name, texture, FAtlasManager._nextAtlasIndex++, false);

            atlasManager.AddAtlas(atlas);

            return atlas;
        }

        public static void LoadElement(string elementName)
        {
            if (Futile.atlasManager.GetAtlasWithName(elementName) != null)
            {
                return;
            }
            string text = AssetManager.ResolveFilePath("Illustrations" + Path.DirectorySeparatorChar.ToString() + elementName + ".png");
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            AssetManager.SafeWWWLoadTexture(ref texture2D, "file:///" + text, false, true);
            Futile.atlasManager.LoadAtlasFromTexture(elementName, texture2D, false);
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

        public static Texture2D SteamImageToTexture2D(int imageHandle)
        {
            if (imageHandle == 0)
            {
                return null;
            }

            // Icon not loaded yet
            if (imageHandle == 0)
                return null;

            bool sizeOk = SteamUtils.GetImageSize(imageHandle, out uint width, out uint height);

            if (!sizeOk || width == 0 || height == 0)
                return null;

            byte[] imageData = new byte[width * height * 4];

            bool imageOk = SteamUtils.GetImageRGBA(
                imageHandle,
                imageData,
                imageData.Length
            );

            if (!imageOk)
                return null;

            FlipVertically(imageData, (int)width, (int)height);

            Texture2D texture = new((int)width, (int)height, TextureFormat.RGBA32, false);

            texture.LoadRawTextureData(imageData);
            texture.Apply();

            return texture;
        }

        private static void FlipVertically(byte[] rgba, int width, int height)
        {
            int rowSize = width * 4;
            byte[] temp = new byte[rowSize];

            for (int y = 0; y < height / 2; y++)
            {
                int top = y * rowSize;
                int bottom = (height - y - 1) * rowSize;

                Buffer.BlockCopy(rgba, top, temp, 0, rowSize);
                Buffer.BlockCopy(rgba, bottom, rgba, top, rowSize);
                Buffer.BlockCopy(temp, 0, rgba, bottom, rowSize);
            }
        }
    }
}