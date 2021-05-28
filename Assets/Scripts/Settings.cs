using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class Settings : Entity
    {
        [Header("App settings")]
        public Slider audioVolumeSlider;
        public Toggle vibrationToggle;
        [Header("Game settings")]
        public Toggle workOnBothToggle;
        public Toggle workOnWordsToggle;
        public Toggle workOnKanjiToggle;

        public Button saveButton;

        private bool saveLoaded = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateUIInputs(GameManager.saveManager.settingsData);
        }

        private void UpdateUIInputs(SettingsData data)
        {
            audioVolumeSlider.value = data.Volume;
            vibrationToggle.isOn = data.EnableVibrations;
            workOnBothToggle.isOn = data.UseWordsAndKanji;
            workOnWordsToggle.isOn = data.UseOnlyWords;
            workOnKanjiToggle.isOn = data.UseOnlyKanji;

            saveLoaded = true;
        }

        public void SelectWorkToggle(int index)
        {
            // 0 = both, 1 = words, 2 = kanji
            switch (index)
            {
                case 0:
                    if(workOnBothToggle.isOn)
                    {
                        workOnWordsToggle.isOn = false;
                        workOnKanjiToggle.isOn = false;
                    }
                    else
                    {
                        if (!workOnKanjiToggle.isOn && !workOnWordsToggle.isOn) workOnBothToggle.isOn = true;
                    }
                    break;
                case 1:
                    if(workOnWordsToggle.isOn)
                    {
                        if(GameManager.saveManager.statsData.WordCards < 5)
                        {
                            GameManager.instance.CallOnInfoPopUpRequested("Not enough words", "You don't have enough word cards to work only on them! You need at least 5 word cards, you currently have " + GameManager.saveManager.statsData.WordCards.ToString(), "Continue");
                            workOnWordsToggle.isOn = false;
                            return;
                        }
                        workOnBothToggle.isOn = false;
                        workOnKanjiToggle.isOn = false;
                    }
                    else
                    {
                        if (!workOnKanjiToggle.isOn && !workOnBothToggle.isOn) workOnWordsToggle.isOn = true;
                    }
                    break;
                case 2:
                    if(workOnKanjiToggle.isOn)
                    {
                        if (GameManager.saveManager.statsData.KanjiCards < 5)
                        {
                            GameManager.instance.CallOnInfoPopUpRequested("Not enough kanji", "You don't have enough kanji cards to work only on them! You need at least 5 kanji cards, you currently have " + GameManager.saveManager.statsData.KanjiCards.ToString(), "Continue");
                            workOnKanjiToggle.isOn = false;
                            return;
                        }
                        workOnBothToggle.isOn = false;
                        workOnWordsToggle.isOn = false;
                    }
                    else
                    {
                        if (!workOnBothToggle.isOn && !workOnWordsToggle.isOn) workOnKanjiToggle.isOn = true;
                    }
                    break;
            }
        }

        public void ActivateSaveButton()
        {
            if (!saveLoaded) return;
            saveButton.interactable = true;
        }

        public void SaveChanges()
        {
            SettingsData newData = new SettingsData(audioVolumeSlider.value, vibrationToggle.isOn, workOnBothToggle.isOn, workOnWordsToggle.isOn, workOnKanjiToggle.isOn, GameManager.saveManager.settingsData.HasFullVersion);
            GameManager.saveManager.settingsData = newData;
            GameManager.saveManager.SaveSettingsData();
            saveButton.interactable = false;
        }

        public void DeleteCards()
        {
            GameManager.instance.CallOnChoicePopUpRequested("Warning!", "You are about to delete all " + GameManager.saveManager.memoryData.Flashcards.Length + " of your flashcards. This can not be undone later. Are you sure you want to continue?", "Yes", GameManager.saveManager.DeleteAllFlashcards, "No", null);
        }

        public void ResetApp() // DEBUG METHOD TO BE DELETED
        {
            GameManager.instance.CallOnChoicePopUpRequested("Warning!", "You are about to reset the app. This means that all your stats, settings and flashcards will be deleted. This can't be undone. (You will not have to buy the full version again)", "Yes", GameManager.saveManager.ResetApp, "No", null);
        }
    }
}
