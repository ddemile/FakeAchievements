using Menu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace FakeAchievements
{
    public class AchievementMenu : Menu.Menu
    {
        public MenuLabel achievementTitle;
        public MenuLabel achievementSubTitle;

        public State state = State.Appearing;

        public const float speedFactor = 1.5f;

        public int shownTime = 0;

        public enum State
        {
            Appearing,
            Showed,
            Disappearing,
            Hidden
        }

        public AchievementMenu(ProcessManager manager, Achievement achievement) : base(manager, new ProcessManager.ProcessID("FakeAchievementMenu", true))
        {
            pages.Add(new Page(this, null, "main", 0));
            PlaySound(Sounds.STEAM_ACHIEVEMENT);

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
                new FSprite(achievement.imageName)
                {
                    y = 13,
                    x = 11,
                    width = 44,
                    height = 44,
                    anchorX = 0,
                    anchorY = 0,
                }
            );

            achievementTitle = new MenuLabel(this, pages[0], achievement.title, new Vector2(70, 36), new Vector2(200, 10), false);
            achievementSubTitle = new MenuLabel(this, pages[0], achievement.description, new Vector2(70, 20 + 15), new Vector2(200, 10), false);
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

            if (state == State.Disappearing)
            {
                this.container.y -= speedFactor;
            }
            else if (shownTime >= 1000)
            {
                this.state = State.Disappearing;
            }
            else if (state == State.Showed)
            {
                shownTime++;
            }
            else if (container.y >= 0)
            {
                this.container.y = 0;
                this.state = State.Showed;
            }
            else if (state == State.Appearing)
            {
                this.container.y += speedFactor;
            }
            else if (container.y <= -69)
            {
                this.container.y = -69;
                this.state = State.Hidden;
                AchievementsManager.menuInstances.Remove(this);
            }

            this.container.MoveToFront();
            this.pages[0].GrafUpdate(timeStacker);
        }
    }
}
