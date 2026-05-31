using Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FakeAchievements
{
    internal class SliderController : PositionedMenuObject, Slider.ISliderOwner
    {
        public float Value { get; set; }
        public bool IsVisible {
            get;
            set
            {
                Container.isVisible = value;
                field = value;
            }
        }

        public SliderController(Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            myContainer = new FContainer();
            owner.Container.AddChild(myContainer);
        }

        public override void Update()
        {
            if (!IsVisible) return;

            base.Update();
        }

        public void SliderSetValue(Slider slider, float setValue)
        {
            if (IsVisible)
            {
                Value = setValue;
            }
        }

        public float ValueOfSlider(Slider slider)
        {
            return Value;
        }
    }
}
