using Menu;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FakeAchievements;

internal class CardsContainer : RectangularMenuObject, Slider.ISliderOwner
{
    internal static Vector2 CAM_POS = new(10000f, -10000f);

    private Camera cam;
    private FTexture insideTexture;
    private RenderTexture renderTexture;
    public float scrollOffset;
    private RoundedRect rectBack;
    private SliderController sliderController;
    private VerticalSlider slider;
    public float contentSize;

    public CardsContainer(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size)
    {
        myContainer = new FContainer();
        myContainer.SetPosition(CAM_POS);

        owner.Container.AddChild(myContainer);

        GameObject gameObject = new GameObject("CardsContainer Camera");
        cam = gameObject.AddComponent<Camera>();

        Vector3 color = Custom.RGB2HSL(MenuColorEffect.rgbMediumGrey);

        rectBack = new(menu, owner, pos, size, true)
        {
            fillAlpha = 0.5f,
            borderColor = new HSLColor(color[0], color[1], color[2])
        };

        subObjects.Add(rectBack);

        sliderController = new(menu, owner, pos)
        {
            Value = 1
        };

        slider = new(menu, sliderController, "", new Vector2(size.x + AchievementsMenu.PADDING, 0), new Vector2(0, size.y - 20f), SliderID, false);

        sliderController.subObjects.Add(slider);

        owner.subObjects.Add(sliderController);

        UpdateCam();
    }

    public readonly Slider.SliderID SliderID = new("AchievementsMenuScroll", true);

    public void AddCard(AchievementCard card)
    {
        subObjects.Add(card);
    }

    void MoveCam()
    {
        Vector3 vector = (Vector3)CAM_POS + new Vector3(size.x / 2f, size.y / 2f, -50f) + Vector3.down * (contentSize - size.y + AchievementsMenu.PADDING * 2) * (1 - sliderController.Value);
        vector += (Vector3)ScreenPos;

        cam.gameObject.transform.position = new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    void UpdateCam()
    {
        cam.aspect = size.x / size.y;
        cam.orthographic = true;
        cam.orthographicSize = size.y / 2f;
        cam.nearClipPlane = 1f;
        cam.farClipPlane = 100f;
        MoveCam();
        cam.depth = -1000f;

        ConsoleWrite(size.x + " " + size.y);
        
        renderTexture = new RenderTexture((int)size.x, (int)size.y, 8)
        {
            filterMode = FilterMode.Point
        };
        cam.targetTexture = renderTexture;
            
        insideTexture = new FTexture(renderTexture, "CardsContainerTexture")
        {
            anchorX = 0f,
            anchorY = 0f,
            x = DrawX(1),
            y = DrawY(1)
        };

        owner.Container.AddChild(insideTexture);

        insideTexture.MoveBehindOtherNode(rectBack.sprites[rectBack.SideSprite(0)]);
    }

    public override void Update()
    {
        bool shouldShow = contentSize + AchievementsMenu.PADDING * 2 > size.y;

        sliderController.IsVisible = shouldShow;

        if (shouldShow && MouseOver && menu.manager.menuesMouseMode && menu.mouseScrollWheelMovement != 0)
        {
            sliderController.Value -= menu.mouseScrollWheelMovement / contentSize * 45;
            sliderController.Value = Mathf.Max(0, Mathf.Min(1, sliderController.Value));
        }
        MoveCam();
        base.Update();
    }

    public override void RemoveSprites()
    {
        UnityEngine.Object.Destroy(cam.gameObject);
        owner.RemoveSubObject(sliderController);
        base.RemoveSprites();
    }

    float Slider.ISliderOwner.ValueOfSlider(Slider slider)
    {
        return 1;
    }

    void Slider.ISliderOwner.SliderSetValue(Slider slider, float setValue)
    {
        return;
    }
}
