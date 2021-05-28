using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class ShopOpenerButton : Entity
    {
        public Button linkedButton;

        public override void OnDatasInitialized()
        {
            base.OnDatasInitialized();
            if(GameManager.saveManager.settingsData.HasFullVersion)
            {
                Destroy(this.gameObject);
            }
        }

        public override void OnMenuChanged(MenuType type)
        {
            base.OnMenuChanged(type);
            if(type == MenuType.Tutorial)
            {
                linkedButton.interactable = false;
            }
            else
            {
                linkedButton.interactable = true;
            }
        }

        public override void OnFullVersionBought()
        {
            base.OnFullVersionBought();
            Destroy(this.gameObject);
        }

        public void OpenShop()
        {
            GameManager.instance.CallOnRequestShopOpen();
        }

    }
}
