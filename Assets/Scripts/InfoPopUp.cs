using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class InfoPopUp : Entity
    {
        [Header("Components")]
        public GameObject groupGO;
        public Text titleText;
        public Text infoText;
        public Text buttonText;
        public Animator anim;

        public override void OnInfoPopUpRequested(string title, string info, string button)
        {
            base.OnInfoPopUpRequested(title, info, button);
            titleText.text = title;
            infoText.text = info;
            buttonText.text = button;
            groupGO.SetActive(true);
            anim.SetTrigger("appear");
        }

        public void ClosePopUp()
        {
            anim.SetTrigger("disappear");
        }

        public void DisablePopUp()
        {
            groupGO.SetActive(false);
        }

    }
}
