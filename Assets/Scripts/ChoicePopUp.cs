using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Otumn.Bokya
{
    public class ChoicePopUp : Entity
    {
        [Header("Components")]
        public GameObject groupGO;
        public Text titleText;
        public Text infoText;
        public Text positiveButtonText;
        public Button positiveButton;
        public Text negativeButtonText;
        public Button negativeButton;
        public Animator anim;

        private Action positiveAct;
        private Action negativeAct;

        public override void OnChoicePopUpRequested(string title, string info, string positiveText, Action positiveAction, string negativeText, Action negativeAction)
        {
            base.OnChoicePopUpRequested(title, info, positiveText, positiveAction, negativeText, negativeAction);
            titleText.text = title;
            infoText.text = info;
            positiveButtonText.text = positiveText;
            positiveAct += positiveAction;
            negativeButtonText.text = negativeText;
            negativeAct += negativeAction;
            groupGO.SetActive(true);
            anim.SetTrigger("appear");
            GameManager.instance.CallOnRequestSound(SoundRequest.Warning, 0.5f);
        }

        public void PositiveAnswer()
        {
            if(positiveAct != null) positiveAct.Invoke();
            ClosePopUp();
        }

        public void NegativeAnswer()
        {
            if(negativeAct != null) negativeAct.Invoke();
            ClosePopUp();
        }

        private void ClosePopUp()
        {
            positiveAct = null;
            negativeAct = null;
            anim.SetTrigger("disappear");
        }

        public void DisablePopUp()
        {
            groupGO.SetActive(false);
        }
    }
}
