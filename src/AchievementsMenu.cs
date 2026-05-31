using FakeAchievements.Enums;
using Menu;
using RWCustom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FakeAchievements
{
    internal class AchievementsMenu : Menu.Menu
    {
        public const int SCREEN_WIDTH = 1368;
        public const int SCREEN_HEIGHT = 770;
        internal const int SPACING = 6;
        internal const int PADDING = 15;

        readonly List<AchievementCard> steamCards = [];
        readonly List<AchievementCard> customCards = [];

        readonly CardsContainer cardsContainer;

        readonly List<FNode> customPageElements = []; 

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

            cardsContainer = new(this, Page,
                new Vector2(
                    SCREEN_WIDTH / 2 - AchievementCard.CARD_WIDTH,
                    SCREEN_HEIGHT * (1f / 5f) * 0.5f
                ) - Vector2.one * PADDING,
                new Vector2(
                    AchievementCard.CARD_WIDTH * 2,
                    SCREEN_HEIGHT * (4f / 5f)
                ) + Vector2.one * PADDING * 2
            );

            Page.subObjects.Add(new SimpleButton(this, Page, "BACK", "BACK", new Vector2(cardsContainer.pos.x, 20), new Vector2(110f, 30f)));

            // Init steam achievements
            List<SteamAchievement> steamAchievements = SteamFetcher.FetchAchievements();

            steamAchievements.Sort((a, b) => (b.Achieved ? 2 : b.Hidden ? 0 : 1) - (a.Achieved ? 2 : a.Hidden ? 0 : 1));

            for (int i = 0; i < steamAchievements.Count; i++)
            {
                SteamAchievement achievement = steamAchievements[i];

                bool isEven = i % 2 == 0;

                AchievementCard card = new(achievement, this, cardsContainer, new(cardsContainer.size.x / 2 - (isEven ? AchievementCard.CARD_WIDTH : 0), PADDING + (AchievementCard.SIZE + SPACING) * (i / 2) * -1f));
                
                steamCards.Add(card);
            }

            // Init custom achievements
            List<Achievement> customAchievements = AchievementsManager.achievements;

            var groupedAchievements = customAchievements.GroupBy(a => a.ModId).ToList();
            groupedAchievements.Sort((a, b) =>
            {
                string nameA = ModManager.ActiveMods.Find(mod => mod.id == a.Key).name;
                string nameB = ModManager.ActiveMods.Find(mod => mod.id == b.Key).name;

                return nameA.CompareTo(nameB);
            });

            float lastY = 0;
            float lastCardY = PADDING;

            int lineBottomPadding = 10;

            foreach (var achievementsGroup in groupedAchievements)
            {
                ModManager.Mod mod = ModManager.ActiveMods.Find(mod => mod.id == achievementsGroup.Key);

                FLabel modLabel = new(Custom.GetDisplayFont(), mod.name)
                {
                    x = cardsContainer.DrawX(1) + cardsContainer.size.x / 2 - AchievementCard.CARD_WIDTH,
                    y = cardsContainer.DrawY(1) + (AchievementCard.SIZE + SPACING) + lastCardY,
                    alignment = FLabelAlignment.Left,
                    anchorX = 0,
                    anchorY = 1,
                    isVisible = false
                };
                
                customPageElements.Add(modLabel);

                FSprite line = new("pixel", true)
                {
                    x = modLabel.x,
                    y = modLabel.y - modLabel.textRect.height - 2,
                    scaleX = AchievementCard.CARD_WIDTH * 2,
                    scaleY = 2,
                    color = MenuColorEffect.rgbMediumGrey,
                    anchorX = 0,
                    _anchorY = 1,
                    isVisible = false
                };

                customPageElements.Add(line);

                lastY = line.y - line.scaleY;

                cardsContainer.myContainer.AddChild(modLabel);
                cardsContainer.myContainer.AddChild(line);

                List<Achievement> achievements = achievementsGroup.Where(achievement => achievement.VisibleInMenu).ToList();

                achievements.Sort((a, b) => (b.Achieved ? 2 : b.Hidden ? 0 : 1) - (a.Achieved ? 2 : a.Hidden ? 0 : 1));

                for (int i = 0; i < achievements.Count; i++)
                {
                    Achievement achievement = achievements[i];

                    bool isEven = i % 2 == 0;

                    AchievementCard card = new(achievement, this, cardsContainer, new(cardsContainer.size.x / 2 - (isEven ? AchievementCard.CARD_WIDTH : 0), lastY - cardsContainer.DrawY(1) - AchievementCard.SIZE - lineBottomPadding - (AchievementCard.SIZE + SPACING) * (i / 2)));
                    lastCardY = card.pos.y - AchievementCard.SIZE - PADDING;
                    
                    customCards.Add(card);
                }
            }

            UpdateCardsContainer(steamCards);

            Page.subObjects.Add(cardsContainer);

            string[] categories = ["STEAM", "CUSTOM"];

            for (int i = 0; i < categories.Length; i++)
            {
                string category = categories[i];

                int width = 30;
                int height = 85;

                var button = new SimpleButton(this, Page, category, category, cardsContainer.pos + new Vector2(-PADDING - width, cardsContainer.size.y - height - (height + PADDING) * i), new Vector2(width, height));
                button.menuLabel.label.rotation = -90;

                Page.subObjects.Add(button);
            }
        }

        public void UpdateCardsContainer(List<AchievementCard> cards)
        {
            cardsContainer.Reset();
            foreach(AchievementCard card in cards)
            {
                card.Show();
                cardsContainer.AddCard(card);
            }

            cardsContainer.myContainer.SetPosition(cardsContainer.myContainer.GetPosition() + new Vector2(0, cardsContainer.size.y - PADDING * 2 - AchievementCard.SIZE));

            cardsContainer.contentSize = -cards.Last().pos.y + AchievementCard.SIZE + PADDING;
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message == "CUSTOM")
            {
                foreach (AchievementCard card in steamCards)
                {
                    card.Hide();
                }
                foreach (FNode node in customPageElements)
                {
                    node.isVisible = true;
                }
                UpdateCardsContainer(customCards);
                PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
            }
            else if (message == "STEAM")
            {
                foreach (AchievementCard card in customCards)
                {
                    card.Hide();
                }
                foreach (FNode node in customPageElements)
                {
                    node.isVisible = false;
                }
                UpdateCardsContainer(steamCards);
                PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
            } else if (message == "BACK")
            {
                OnExit();
            }

            base.Singal(sender, message);
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

        public AchievementCard(Achievement achievement, Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            bool visible = achievement.Achieved || !achievement.Hidden;

            title = new(Custom.GetDisplayFont(), visible ? achievement.Title : "Hidden achievement")
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

            image = new FSprite(visible ? achievement.ImageName : "multiplayerportrait02")
            {
                x = int.MinValue,
                y = int.MinValue,
                width = SIZE,
                height = SIZE,
                anchorX = 0,
                anchorY = 0,
                color = (achievement.HasLockedImage || achievement.Achieved || achievement.Hidden) ? Color.white : MenuColorEffect.rgbDarkGrey
            };

            if (!achievement.HasLockedImage && !achievement.Achieved && !achievement.Hidden)
            {
                image.color = MenuColorEffect.rgbDarkGrey;
                image.shader = Custom.rainWorld.Shaders["FAGreyscale"];
            }

            Container.AddChild(image);
            Container.AddChild(title);
            Container.AddChild(subTitle);
        }

        public AchievementCard(SteamAchievement achievement, Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            bool visible = achievement.Achieved || !achievement.Hidden;

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

        public void Hide()
        {
            image.isVisible = false;
            title.isVisible = false;
            subTitle.isVisible = false;
        }

        public void Show()
        {
            image.isVisible = true;
            title.isVisible = true;
            subTitle.isVisible = true;
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
