using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class MenuButton : Entity
    {
        public MenuType ownType;
        public Button linkedButton;
        public Image icon;
        public Image background;
        public Text text;
        public Color selectedColor;

        public override void OnDatasInitialized()
        {
            base.OnDatasInitialized();
        }

        public void SelectMenu()
        {
            GameManager.instance.CallOnMenuChanged(ownType);
            GameManager.instance.CallOnRequestSound(SoundRequest.Click, 0.35f);
        }

        public override void OnMenuChanged(MenuType type)
        {
            base.OnMenuChanged(type);
            if (type == MenuType.Tutorial)
            {
                linkedButton.interactable = false;
                background.color = GameManager.resources.SelectedOppositeColor;
                return;
            }
            if(ownType != type)
            {
                linkedButton.interactable = true;
                background.color = GameManager.resources.BaseBlue;
                icon.color = Color.white;
                text.color = Color.white;
            }
            else
            {
                linkedButton.interactable = false;
                icon.color = GameManager.resources.SelectedColor;
                text.color = GameManager.resources.SelectedColor;
                background.color = GameManager.resources.SelectedBlue;
            }
        }
    }
}