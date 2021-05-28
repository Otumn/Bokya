using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class Tutorial : Entity
    {
        public GameObject[] slides;
        public Button nextSlideButton;
        public Button previousSlideButton;
        public Button validateTutorialButton;
        public AnimationCurve slideCurve;
        public float slideSpeed = 200f;

        private int currentSlideIndex = 0;
        private int targetSlideIndex = 0;
        private float slideIncremeneter = 0f;
        private bool isSliding = false;
        private Vector2 lastSlidePos;
        private Vector2 lastSlideTargetPos;
        private Vector2 targetSlideInitialPos;

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInteractableButtons();
        }

        protected override void Update()
        {
            base.Update();
            SlideManagement();
        }

        public void JumpSlide(int increment)
        {
            targetSlideIndex = Mathf.Clamp(currentSlideIndex + increment, 0, slides.Length - 1);
            if (targetSlideIndex == currentSlideIndex) return;

            lastSlidePos = slides[currentSlideIndex].transform.position;
            if(targetSlideIndex > currentSlideIndex)
            {
                lastSlideTargetPos = new Vector2(lastSlidePos.x - Screen.width, lastSlidePos.y);
                targetSlideInitialPos = new Vector2(lastSlidePos.x + Screen.width, lastSlidePos.y);
            }
            else
            {
                lastSlideTargetPos = new Vector2(lastSlidePos.x + Screen.width, lastSlidePos.y);
                targetSlideInitialPos = new Vector2(lastSlidePos.x - Screen.width, lastSlidePos.y);
            }
            slides[targetSlideIndex].transform.position = targetSlideInitialPos;
            slides[targetSlideIndex].SetActive(true);

            previousSlideButton.interactable = false;
            nextSlideButton.interactable = false;
            isSliding = true;
            GameManager.instance.CallOnRequestSound(SoundRequest.Whoosh, 0.35f);
        }

        private void SlideManagement()
        {
            if(isSliding)
            {
                slides[currentSlideIndex].transform.position = Vector3.Lerp(lastSlidePos, lastSlideTargetPos, slideCurve.Evaluate(slideIncremeneter));
                slides[targetSlideIndex].transform.position = Vector3.Lerp(targetSlideInitialPos, lastSlidePos, slideCurve.Evaluate(slideIncremeneter));

                slideIncremeneter += Time.deltaTime * slideSpeed;
                if(slideIncremeneter >= 1f)
                {
                    slides[targetSlideIndex].transform.position = lastSlidePos;
                    slides[currentSlideIndex].SetActive(false);
                    currentSlideIndex = targetSlideIndex;
                    CheckInteractableButtons();
                    slideIncremeneter = 0f;
                    isSliding = false;
                }
            }
        }

        public void ValidateTutorial()
        {
            GameManager.saveManager.settingsData.TutorialDone = true;
            GameManager.saveManager.SaveSettingsData();
            GameManager.instance.CallOnMenuChanged(MenuType.Memory);
        }

        private void CheckInteractableButtons()
        {
            if(currentSlideIndex == slides.Length - 1)
            {
                validateTutorialButton.interactable = true;
            }
            if(currentSlideIndex == 0)
            {
                previousSlideButton.interactable = false;
                nextSlideButton.interactable = true;
            }
            else if(currentSlideIndex > 0 && currentSlideIndex < slides.Length - 1)
            {
                previousSlideButton.interactable = true;
                nextSlideButton.interactable = true;
            }
            else if(currentSlideIndex == slides.Length - 1)
            {
                previousSlideButton.interactable = true;
                nextSlideButton.interactable = false;
            }
        }

    }
}
