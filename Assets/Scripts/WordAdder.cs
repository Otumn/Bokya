using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class WordAdder : Entity
    {
        public GameObject wordCanvas;
        public GameObject kanjiCanvas;

        public Text TitleText;
        public Text addWordButton;

        #region WordAdder
        [Header("Word adder")]
        public InputField kanjiWordField;
        public InputField kanaField;
        public InputField tongueField;
        #endregion

        #region KanjiAdder
        [Header("Kanji adder")]
        public InputField kanjiField;
        public InputField hiraField;
        public InputField kataField;
        public InputField meaningField;
        #endregion

        private bool IsValidationPossible()
        {
            if(kanjiCanvas.activeSelf) // check for kanji adding conditions
            {
                for (int i = 0; i < GameManager.saveManager.memoryData.Flashcards.Length; i++)
                {
                    if (GameManager.saveManager.memoryData.Flashcards[i].KanjiWord == kanjiField.text)
                    {
                        GameManager.instance.CallOnInfoPopUpRequested("Error", "A flash card with this kanji is already saved.", "Ok");
                        return false;
                    }
                }
                if (kanjiField.text == "")
                {
                    GameManager.instance.CallOnInfoPopUpRequested("Error", "Please fill the kanji field to add the card.", "Ok");
                    return false;
                }
                if (hiraField.text == "" && kataField.text == "")
                {
                    GameManager.instance.CallOnInfoPopUpRequested("Error", "Please fill the hiragana field to add the card.", "Ok");
                    return false;
                }
                if (kataField.text == "")
                {
                    GameManager.instance.CallOnInfoPopUpRequested("Error", "Please fill the katakana field to add the card.", "Ok");
                    return false;
                }
                if (meaningField.text == "")
                {
                    GameManager.instance.CallOnInfoPopUpRequested("Error", "Please fill the meaning field to add the card.", "Ok");
                    return false;
                }
                return true;
            }
            else // check for word adding conditions
            {
                for (int i = 0; i < GameManager.saveManager.memoryData.Flashcards.Length; i++)
                {
                    if (GameManager.saveManager.memoryData.Flashcards[i].KanjiWord == kanjiWordField.text)
                    {
                        GameManager.instance.CallOnInfoPopUpRequested("Error", "A flash card with this kanji writing is already saved.", "Ok");
                        return false;
                    }
                }
                if (kanjiWordField.text == "")
                {
                    GameManager.instance.CallOnInfoPopUpRequested("Error", "Please fill the kanji field to add the card.", "Ok");
                    return false;
                }
                if (kanaField.text == "")
                {
                    GameManager.instance.CallOnInfoPopUpRequested("Error", "Please fill the kana field to add the card.", "Ok");
                    return false;
                }
                if (tongueField.text == "")
                {
                    GameManager.instance.CallOnInfoPopUpRequested("Error", "Please fill the mothertongue field to add the card.", "Ok");
                    return false;
                }
            }

            if(!GameManager.saveManager.settingsData.HasFullVersion && GameManager.saveManager.memoryData.Flashcards.Length >= 40)
            {
                GameManager.instance.CallOnChoicePopUpRequested("Full version", "You are limited to 40 flashcards with the free version of the app. Purchase the full version to add as many as you want.", "Go to shop", GameManager.instance.CallOnRequestShopOpen, "Continue", null);
                return false;
            }
            return true;
        }

        public void ValidateWordEntry()
        {
            if(!IsValidationPossible())
            {
                Debug.Log("Validation not possible");
                return;
            }

            Flashcard addedCard = new Flashcard();
            #region New flashcard construction
            addedCard.IsKanjiCard = kanjiCanvas.activeSelf;
            if(kanjiCanvas.activeSelf)
            {
                addedCard.KanjiWord = kanjiField.text;
                addedCard.MotherTongueWord = meaningField.text;
            }
            else
            {
                addedCard.KanjiWord = kanjiWordField.text;
                addedCard.MotherTongueWord = tongueField.text;
            }
            addedCard.HiraganaWord = hiraField.text;
            addedCard.KatakanaWord = kataField.text;
            addedCard.KanaWord = kanaField.text;
            System.DateTime today = System.DateTime.Today;
            addedCard.SeenDay = today.Day;
            addedCard.SeenMonth = today.Month;
            addedCard.SeenYear = today.Year;
            #endregion

            #region Adding new flashcard to saved array
            List<Flashcard> tempList = new List<Flashcard>(GameManager.saveManager.memoryData.Flashcards);
            addedCard.Index = tempList.Count;
            tempList.Add(addedCard);
            GameManager.saveManager.memoryData.Flashcards = tempList.ToArray();
            GameManager.saveManager.SaveMemoryData();
            GameManager.saveManager.LoadMemoryData();
            #endregion

            #region Clearing all input fields
            kanjiWordField.text = "";
            kanaField.text = "";
            tongueField.text = "";
            kanjiField.text = "";
            hiraField.text = "";
            kataField.text = "";
            meaningField.text = "";
            #endregion
        }

        public void ToggleKanjiAdder()
        {
            if(wordCanvas.activeSelf)
            {
                addWordButton.text = "Kanji card";
                wordCanvas.SetActive(false);
                kanjiCanvas.SetActive(true);
            }
            else
            {
                addWordButton.text = "Word card";
                wordCanvas.SetActive(true);
                kanjiCanvas.SetActive(false);
            }
        }
    }
}