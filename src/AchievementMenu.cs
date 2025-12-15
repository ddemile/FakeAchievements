using System;
using System.Collections.Generic;
using FakeAchievements.Enums;
using Menu;
using UnityEngine;
using AchievementRequest = (FakeAchievements.Achievement Achievement, float Delay);

namespace FakeAchievements
{
    public class AchievementMenu : Menu.Menu
    {
        public enum State
        {
            Appearing,
            Shown,
            Disappearing,
            Hidden
        }

        private const float SpeedFactor = 1.5f;
        private const int MaxDisplayedAchievements = 3;

        private static readonly List<AchievementRequest> displayRequests = [];
        private static readonly List<AchievementMenu> activeSlots = [];

        private readonly MenuLabel achievementTitle;
        private readonly MenuLabel achievementSubTitle;

        private readonly List<FNode> childNodes = [];

        private State state = State.Appearing;
        private int shownTime;

        private float delayBeforeVisible;
        private bool initialized;

        private int slotIndex = 1;
        private int yFactor;

        private float? moveTowardsY;

        public AchievementMenu(ProcessManager manager, Achievement achievement) : base(manager, ProcessIDs.FakeAchievementMenu)
        {
            pages.Add(new Page(this, null, "main", 0));

            container.x = Plugin.RW.options.ScreenSize.x - (282 - 1);
            container.y = -69;

            childNodes.AddRange([
                new FSprite("illustrations/achievement_background")
                {
                    x = 0,
                    y = 0,
                    width = 282,
                    height = 69,
                    anchorX = 0,
                    anchorY = 0,
                },
                new FSprite(achievement.ImageName)
                {
                    x = 11,
                    y = 13,
                    width = 44,
                    height = 44,
                    anchorX = 0,
                    anchorY = 0,
                }
            ]);

            container.AddChild(childNodes[0]);
            container.AddChild(childNodes[1]);

            achievementTitle = new MenuLabel(this, pages[0], achievement.Title, new Vector2(70, 36), new Vector2(200, 10), false);
            achievementSubTitle = new MenuLabel(this, pages[0], achievement.Description, new Vector2(70, 20 + 15), new Vector2(200, 10), false);

            achievementSubTitle.label.color = Color.gray;

            achievementTitle.label.alignment = FLabelAlignment.Left;
            achievementSubTitle.label.alignment = FLabelAlignment.Left;

            achievementTitle.label.anchorY = 0;
            achievementSubTitle.label.anchorY = 1;

            Utils.WordWrapLabel(achievementTitle.label, achievementTitle.size.x, 1);
            Utils.WordWrapLabel(achievementSubTitle.label, achievementSubTitle.size.x, 2);
        }

        public override void GrafUpdate(float timeStacker)
        {
            if (delayBeforeVisible > 0f)
            {
                delayBeforeVisible = Mathf.Max(delayBeforeVisible - timeStacker, 0f);
                return;
            }

            if (achievementTitle != null && achievementSubTitle != null)
            {
                achievementTitle.label.x = achievementTitle.DrawX(timeStacker);
                achievementTitle.label.y = achievementTitle.DrawY(timeStacker);

                achievementSubTitle.label.x = achievementSubTitle.DrawX(timeStacker);
                achievementSubTitle.label.y = achievementSubTitle.DrawY(timeStacker);
            }

            switch (state)
            {
                case State.Appearing:
                    {
                        if (!initialized)
                        {
                            foreach (AchievementMenu slot in activeSlots)
                            {
                                if (slot == this || !slot.initialized) continue;

                                slot.UpdateSlotIndex(activeSlots.IndexOf(slot) + 1);
                            }

                            PlaySound(SoundIDs.STEAM_ACHIEVEMENT);
                            initialized = true;
                        }

                        container.y += SpeedFactor;

                        if (container.y >= yFactor) // goalHeight
                        {
                            container.y = yFactor;
                            state = State.Shown;
                        }
                    }
                    break;
                case State.Shown:
                    {
                        shownTime++;

                        if (shownTime >= 1000)
                            state = State.Disappearing;
                    }
                    break;
                case State.Disappearing:
                    {
                        container.y -= SpeedFactor;

                        int disappearHeight = -69 + yFactor;
                        if (container.y <= disappearHeight)
                        {
                            container.y = disappearHeight;
                            state = State.Hidden;

                            Destroy();
                        }
                    }
                    break;
                case State.Hidden:
                default:
                    break;
            }

            if (moveTowardsY is not null)
            {
                container.y = Mathf.MoveTowards(container.y, moveTowardsY.Value, SpeedFactor);

                if (container.y >= moveTowardsY.Value)
                {
                    container.y = moveTowardsY.Value;
                    moveTowardsY = null;
                }
            }

            container.MoveToFront();
            pages[0].GrafUpdate(timeStacker);
        }

        public void Destroy()
        {
            if (!activeSlots.Remove(this))
            {
                Plugin.Log("WARNING: Menu instance destroyed but not found in active instances!");
            }

            Plugin.Log("Destroying achievement menu!");

            foreach (FNode node in childNodes)
            {
                container.RemoveChild(node);
            }

            childNodes.Clear();

            achievementTitle.RemoveSprites();
            achievementSubTitle.RemoveSprites();

            if (activeSlots.Count == 0) return;

            foreach (AchievementMenu slot in activeSlots)
            {
                if (slot.delayBeforeVisible > 0f) continue;

                slot.UpdateSlotIndex(activeSlots.IndexOf(slot) + 1);
            }
        }

        private void UpdateSlotIndex(int newIndex)
        {
            int previousIndex = slotIndex;

            slotIndex = newIndex;
            yFactor = 69 * (newIndex - 1);

            container.sortZ = newIndex;

            moveTowardsY = previousIndex > newIndex
                ? container.y - (69 * (previousIndex - newIndex))
                : container.y + (69 * (newIndex - previousIndex));

            foreach (FNode node in childNodes)
            {
                node.sortZ = newIndex;
            }

            achievementTitle.label.sortZ = newIndex;
            achievementSubTitle.label.sortZ = newIndex;
        }

        public static void RequestMenu(Achievement achievement, float delay = 0f)
        {
            if (float.IsNaN(delay) || float.IsInfinity(delay))
                delay = 0f;

            displayRequests.Add(new AchievementRequest(achievement, delay));
        }

        internal static void ClearInstances()
        {
            displayRequests.Clear();

            if (activeSlots.Count > 0)
            {
                foreach (var slot in activeSlots)
                {
                    try
                    {
                        slot.Destroy();
                    }
                    catch (Exception ex)
                    {
                        Plugin.LogError("Failed to destroy menu instance with ClearInstances()!");
                        Plugin.LogError(ex);
                    }
                }
            }

            activeSlots.Clear();
        }

        internal static void UpdateInstances(float timeStacker)
        {
            if (activeSlots.Count > 0)
            {
                for (int i = activeSlots.Count - 1; i >= 0; i--)
                {
                    activeSlots[i].GrafUpdate(timeStacker);
                }
            }

            if (activeSlots.Count < MaxDisplayedAchievements && displayRequests.Count > 0)
            {
                (Achievement achievement, float delay) = displayRequests[0];

                AchievementMenu instance = new(Plugin.RW.processManager, achievement)
                {
                    delayBeforeVisible = delay,
                };

                activeSlots.Insert(0, instance);

                displayRequests.RemoveAt(0);
            }
        }
    }
}