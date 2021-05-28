using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class CardFocus : Entity
    {
        [Header("Components")]
        public Animator anim;
        [Header("Card informations")]
        public Text cardType;
        public Text cardKanji;
        public Text cardHira;
        public Text cardKata;
        public Text cardKana;
        public Text titleHira;
        public Text titleKata;
        public Text titleKana;
        public Text cardMeaning;
        public Slider cardMastery;
        public Image sliderFiller;

        private int linkedIndex;

        public override void OnFocusOnCard(Flashcard focusedCard)
        {
            base.OnFocusOnCard(focusedCard);
            Initialize(focusedCard);
            anim.SetTrigger("appear");
        }

        public void Initialize(Flashcard card)
        {
            linkedIndex = card.Index;

            cardKanji.text = card.KanjiWord;
            cardHira.text = card.HiraganaWord;
            cardKata.text = card.KatakanaWord;
            cardKana.text = card.KanaWord;
            cardMeaning.text = card.MotherTongueWord;
            if(card.IsKanjiCard)
            {
                cardType.text = "Kanji card";
                titleKana.gameObject.SetActive(false);
                titleHira.gameObject.SetActive(true);
                titleKata.gameObject.SetActive(true);
            }
            else
            {
                cardType.text = "Word card";
                titleKana.gameObject.SetActive(true);
                titleHira.gameObject.SetActive(false);
                titleKata.gameObject.SetActive(false);
            }
            cardMastery.value = card.MasteryScore;
            sliderFiller.color = Color.Lerp(GameManager.resources.BeginnerColor, GameManager.resources.MasteryColor, card.MasteryScore);
        }

        public void CloseFocus()
        {
            anim.SetTrigger("disappear");
        }

        public void RequestDeletion()
        {
            GameManager.instance.CallOnChoicePopUpRequested("Warning", "You are about to delete this card. Are you sure you want to continue?", "Yes", DeleteLinkedCard, "No", null);
        }

        private void DeleteLinkedCard()
        {
            GameManager.saveManager.DeleteFlashcard(linkedIndex);
            CloseFocus();
            GameManager.instance.CallOnFlashcardDeleted();
        }

    }
}
