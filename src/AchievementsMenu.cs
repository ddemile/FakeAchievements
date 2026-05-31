using FakeAchievements.Enums;
using Menu;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace FakeAchievements
{
    internal class AchievementsMenu : Menu.Menu
    {
        public const int SCREEN_WIDTH = 1368;
        public const int SCREEN_HEIGHT = 770;
        internal const int SPACING = 6;
        internal const int PADDING = 15;

        readonly List<AchievementCard> cards = [];

        public AchievementsMenu(ProcessManager manager) : base(manager, ProcessIDs.AchievementsMenu)
        {
            pages.Add(new Page(this, null, "main", 0));

            scene = new InteractiveMenuScene(this, Page, ModManager.MMF ? manager.rainWorld.options.subBackground : MenuScene.SceneID.Landscape_SU);
            Page.subObjects.Add(scene);

            darkSprite = new FSprite("pixel", true);
            darkSprite.color = new Color(0f, 0f, 0f);
            darkSprite.anchorX = 0f;
            darkSprite.anchorY = 0f;
            darkSprite.scaleX = SCREEN_WIDTH;
            darkSprite.scaleY = SCREEN_HEIGHT;
            darkSprite.x = -1f;
            darkSprite.y = -1f;
            darkSprite.alpha = 0.85f;
            Page.Container.AddChild(darkSprite);

            List<SteamAchievement> steamAchievements = SteamFetcher.FetchAchievements();

            steamAchievements.Sort((a, b) => (b.Achieved ? 2 : b.Hidden ? 0 : 1) - (a.Achieved ? 2 : a.Hidden ? 0 : 1));

            CardsContainer container = new(this, Page,
                new Vector2(
                    SCREEN_WIDTH / 2 - AchievementCard.CARD_WIDTH,
                    SCREEN_HEIGHT * (1f / 5f) * 0.5f
                ) - Vector2.one * PADDING,
                new Vector2(
                    AchievementCard.CARD_WIDTH * 2,
                    SCREEN_HEIGHT * (4f / 5f)
                ) + Vector2.one * PADDING * 2
            );

            for (int i = 0; i < steamAchievements.Count; i++)
            {
                SteamAchievement achievement = steamAchievements[i];

                bool isEven = i % 2 == 0;

                AchievementCard card = new(achievement, this, container, new(container.size.x / 2 - (isEven ? AchievementCard.CARD_WIDTH : 0), PADDING + (AchievementCard.SIZE + SPACING) * (i / 2) * -1f));
                
                cards.Add(card);
                container.AddCard(card);
            }

            float startY = cards.First().pos.y + AchievementCard.SIZE;
            float endY = cards.Last().pos.y;

            container.myContainer.SetPosition(container.myContainer.GetPosition() + new Vector2(0, container.size.y - PADDING * 2 - AchievementCard.SIZE));

            container.contentSize = startY - endY;

            Page.subObjects.Add(container);

            string[] categories = ["CUSTOM", "STEAM"];

            for (int i = 0; i < categories.Length; i++)
            {
                string category = categories[i];

                int width = 30;
                int height = 85;

                var button = new SimpleButton(this, Page, category, category, container.pos + new Vector2(-PADDING - width, container.size.y - height - (height + PADDING) * i), new Vector2(width, height));
                button.menuLabel.label.rotation = -90;

                Page.subObjects.Add(button);
            }
        }

        public override void Update()
        {
            bool pressed = RWInput.CheckPauseButton(0);
            if (pressed && !lastPauseButton && manager.dialog == null)
            {
                OnExit();
            }
            lastPauseButton = pressed;
            base.Update();
        }

        public void OnExit()
        {
            if (exiting)
            {
                return;
            }
            exiting = true;
            manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
            PlaySound(SoundID.MENU_Switch_Page_Out);
        }

        private FSprite darkSprite;
        private bool lastPauseButton;
        private bool exiting;

        private Page Page => pages[0];
    }

    internal class AchievementCard : PositionedMenuObject
    {
        public const int SIZE = 64;
        public const float CARD_WIDTH = AchievementsMenu.SCREEN_WIDTH / 2 * (3.6f / 5f);
        const int GAP = 6;

        readonly FLabel title;
        readonly FLabel subTitle;
        readonly FSprite image;

        public AchievementCard(SteamAchievement achievement, Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            bool visible = !achievement.Hidden || achievement.Achieved;

            title = new(Custom.GetDisplayFont(), visible ? achievement.DisplayName : "Hidden achievement")
            {
                x = int.MinValue,
                y = int.MinValue,
                alignment = FLabelAlignment.Left,
                anchorX = 0,
            };

            Utils.WordWrapLabel(title, CARD_WIDTH - SIZE - GAP, 1);

            subTitle = new(Custom.GetFont(), visible ? achievement.Description : "Keep playing to earn this achievement")
            {
                x = int.MinValue,
                y = int.MinValue,
                alignment = FLabelAlignment.Left,
                color = MenuColorEffect.rgbMediumGrey,
                anchorX = 0,
                anchorY = 1
            };

            Utils.WordWrapLabel(subTitle, CARD_WIDTH - SIZE - GAP, 2);

            string spriteName = $"achievement_{achievement.ApiName}";

            if (visible) Utils.LoadSteamImage(spriteName, achievement.Icon);
            else Utils.LoadElement("multiplayerportrait02");

            image = new FSprite(visible ? spriteName : "multiplayerportrait02")
            {
                x = int.MinValue,
                y = int.MinValue,
                width = SIZE,
                height = SIZE,
                anchorX = 0,
                anchorY = 0
            };

            Container.AddChild(image);
            Container.AddChild(title);
            Container.AddChild(subTitle);
        }

        public override void GrafUpdate(float timeStacker)
        {
            image.SetPosition(DrawPos(timeStacker));

            title.x = DrawX(timeStacker) + SIZE + GAP;
            title.y = DrawY(timeStacker) + SIZE * (4f / 5f);

            subTitle.x = DrawX(timeStacker) + SIZE + GAP;
            subTitle.y = DrawY(timeStacker) + SIZE * (3f / 5f);
            base.GrafUpdate(timeStacker);
        }
    }
}
