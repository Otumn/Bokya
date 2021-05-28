using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class CircleProgressBar : Entity
    {
        public Animator anim;
        public Image fillerImage;
        public Image hiderImage;
        public Text infoText;
        public Text percentageText;
        public AnimationCurve fillingCurve;
        public float fillingSpeed = 2f;

        private float fillingIncrementer = 0f;
        private float lastFillAmount;
        private float targetFillAmount;
        private bool isFilling = false;

        protected override void Update()
        {
            base.Update();
            ManageFilling();
        }

        public void SetInfoText(string info)
        {
            infoText.text = info;
        }

        public void StartFilling(float target)
        {
            fillingIncrementer = 0f;
            lastFillAmount = fillerImage.fillAmount;
            targetFillAmount = target;
            isFilling = true;
        }

        public void StopFilling()
        {
            fillerImage.fillAmount = Mathf.Clamp(fillerImage.fillAmount, 0f, 1f);
            UpdateInfos();
            if (fillerImage.fillAmount >= 0.99f)
            {
                anim.SetTrigger("fulled");
                fillerImage.color = GameManager.resources.FullMasterColor;
                Debug.Log("fulled");
            }
            fillingIncrementer = 0f;
            isFilling = false;
        }

        public void ResetValues()
        {
            fillerImage.fillAmount = 0f;
            UpdateInfos();
        }

        private void UpdateInfos()
        {
            fillerImage.color = Color.Lerp(GameManager.resources.BeginnerColor, GameManager.resources.MasteryColor, fillerImage.fillAmount);
            percentageText.text = (fillerImage.fillAmount * 100f).ToString("F0") + "%";
        }

        private void ManageFilling()
        {
            if(isFilling)
            {
                fillerImage.fillAmount = Mathf.Lerp(lastFillAmount, targetFillAmount, fillingCurve.Evaluate(fillingIncrementer));
                UpdateInfos();
                fillingIncrementer += fillingSpeed * Time.deltaTime;
                if(fillingIncrementer > 1f)
                {
                    StopFilling();
                }
            }
        }

        #region Debug methods
        [ContextMenu("Fill to random")]
        public void FillToRand()
        {
            StartFilling(Random.Range(0, 1f));
        }
    
        [ContextMenu("UpdateColor")]
        public void ManuallyUpdateColor()
        {
            UpdateInfos();
        }
        #endregion

    }
}
