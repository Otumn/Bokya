using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class WordPrefab : Entity
    {
        public Text kanjiText;
        public Text meaningText;
        public Animator anim;
        public CircleProgressBar bar;

        private int linkedIndex;
        private float linkedMastery;

        public void Initialize(string kanji, string meaning, float timeBeforeAnim, int index, float mastery)
        {
            kanjiText.text = kanji;
            meaningText.text = meaning;
            linkedIndex = index;
            linkedMastery = mastery;
            StartCoroutine(DelayBeforeAnim(timeBeforeAnim));
        }

        public void ShortCutToAnim(float time)
        {
            StartCoroutine(DelayBeforeAnim(time));
        }

        private IEnumerator DelayBeforeAnim(float time)
        {
            yield return new WaitForSeconds(time);
            anim.SetTrigger("appear");
            bar.StartFilling(linkedMastery);
        }

        public void OpenFocus()
        {
            GameManager.instance.CallOnFocusOnCard(GameManager.saveManager.memoryData.Flashcards[linkedIndex]);
        }
    }
}
