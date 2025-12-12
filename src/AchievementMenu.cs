using System.Collections.Generic;
using FakeAchievements.Enums;
using Menu;
using UnityEngine;
using AchievementRequest = (FakeAchievements.AchievementMenu Instance, float Delay);

namespace FakeAchievements
{
    public class AchievementMenu : Menu.Menu
    {
        public enum State
        {
            Appearing,
            Showed,
            Disappearing,
            Hidden
        }

        private const float SpeedFactor = 1.5f;

        private static readonly List<AchievementRequest> waitingInstances = [];
        private static AchievementMenu activeInstance;

        private static float Delay;

        private readonly MenuLabel achievementTitle;
        private readonly MenuLabel achievementSubTitle;

        private State state = State.Appearing;

        private int shownTime = 0;
        private bool initialized;

        public AchievementMenu(ProcessManager manager, Achievement achievement) : base(manager, ProcessIDs.FakeAchievementMenu)
        {
            pages.Add(new Page(this, null, "main", 0));

            container.y = -69;
            container.x = Plugin.RW.options.ScreenSize.x - (282 - 1);

            container.AddChild(
                new FSprite("illustrations/achievement_background")
                {
                    x = 0,
                    y = 0,
                    width = 282,
                    height = 69,
                    anchorX = 0,
                    anchorY = 0,
                }
            );

            container.AddChild(
                new FSprite(achievement.ImageName)
                {
                    y = 13,
                    x = 11,
                    width = 44,
                    height = 44,
                    anchorX = 0,
                    anchorY = 0,
                }
            );

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
                            PlaySound(SoundIDs.STEAM_ACHIEVEMENT);
                            initialized = true;
                        }

                        container.y += SpeedFactor;

                        if (container.y >= 0)
                        {
                            container.y = 0;
                            state = State.Showed;
                        }
                    }
                    break;
                case State.Showed:
                    {
                        shownTime++;

                        if (shownTime >= 1000)
                            state = State.Disappearing;
                    }
                    break;
                case State.Disappearing:
                    {
                        container.y -= SpeedFactor;

                        if (container.y <= -69)
                        {
                            container.y = -69;
                            state = State.Hidden;

                            activeInstance = null;
                        }
                    }
                    break;
                case State.Hidden:
                default:
                    break;
            }

            container.MoveToFront();
            pages[0].GrafUpdate(timeStacker);
        }

        public static void RequestMenu(Achievement achievement, float delay = 0f)
        {
            AchievementMenu instance = new(Plugin.RW.processManager, achievement);

            if (float.IsNaN(delay) || float.IsInfinity(delay))
                delay = 0f;

            waitingInstances.Add(new AchievementRequest(instance, delay));
        }

        internal static void UpdateInstances(float timeStacker)
        {
            if (Delay > 0f)
            {
                Delay = Mathf.Max(Delay - timeStacker, 0f);
            }
            else if (activeInstance is not null)
            {
                activeInstance.GrafUpdate(timeStacker);
            }
            else if (waitingInstances.Count > 0)
            {
                (AchievementMenu instance, float delay) = waitingInstances[0];

                activeInstance = instance;
                Delay = delay;

                waitingInstances.RemoveAt(0);
            }
        }
    }
}