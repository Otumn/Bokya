using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class Memory : Entity
    {
        [Header("Component")]
        public GameObject choiceGO;
        public GameObject gameKAGO;
        public GameObject gameFKGO;
        public GameObject notEnoughGO;
        public GameObject goBackButton;
        public GameObject freeVersionReminderGO;
        public Animator gameKACanvasAnim;
        public Animator gameFKCanvasAnim;
        [Header("Texts")]
        public Text testedKanjiText;
        public Text clueOneText;
        public Text clueTwoText;
        public Text instructionText;
        public Text notEnoughWordsText;

        public Text testedWordText;
        public Text answerOneFK;
        public Text clueOneFK;
        public Text answerTwoFK;
        public Text clueTwoFK;
        public Text answerThreeFK;
        public Text clueThreeFK;

        [Header("Answer")]
        public InputField answerField;
        public Text answerTextKA;
        public Animator answerAnimKA;
        public Text answerTextFK;
        public Animator answerAnimFK;
        [Header("MasterySliders")]
        public Slider masterySliderKA;
        public Image masteryFillerKA;
        public Slider masterySliderFK;
        public Image masteryFillerFK;
        [Header("Searching parameters")]
        public int numberOfPossibleWords = 3;
        public int turnsBeforeSameWord = 3;

        private int currentWinStreak = 0;
        private bool hasValidated = false;
        private string expectedKAAnswer;
        private int expectedFKAnswer;
        private Flashcard testedCard;
        private Flashcard firstFalseAnswerFK;
        private Flashcard secondFalseAnswerFK;

        private List<int> possibleIndexes = new List<int>();
        private List<IntTracker> recentIndexes = new List<IntTracker>();


        #region Monobehaviour calls
        protected override void OnEnable()
        {
            base.OnEnable();
            //SetupMemoryMenu();
        }
        #endregion

        #region Entity calls
        public override void OnDatasInitialized()
        {
            base.OnDatasInitialized();
            SetupMemoryMenu();
        }

        public override void OnMenuChanged(MenuType type)
        {
            base.OnMenuChanged(type);
            if(type == MenuType.Memory)
            {
                SetupMemoryMenu();
            }
        }

        public override void OnFullVersionBought()
        {
            base.OnFullVersionBought();
            freeVersionReminderGO.SetActive(false);
        }
        #endregion

        #region General functions
        private void SetupMemoryMenu()
        {
            if (GameManager.saveManager.statsData != null) currentWinStreak = GameManager.saveManager.statsData.TempStreak;

            if (GameManager.saveManager.memoryData.Flashcards.Length >= 5)
            {
                if(gameFKGO.activeSelf)
                {
                    DisplayFKGame();
                    notEnoughGO.SetActive(false);
                }
                else if(gameKAGO.activeSelf)
                {
                    DisplayKAGame();
                    notEnoughGO.SetActive(false);
                }
                else
                {
                    DisplayChoiceCanvas();
                }
            }
            else
            {
                gameKAGO.SetActive(false);
                gameFKGO.SetActive(false);
                choiceGO.SetActive(false);
                notEnoughGO.SetActive(true);
                notEnoughWordsText.text = "You need at least 5 words to play, you currently have " + GameManager.saveManager.memoryData.Flashcards.Length;
            }
        }

        private void FillMasterySlider(Slider slider, Image fill)
        {
            slider.value = Mathf.Clamp(testedCard.MasteryScore, 0.09f, 1f);
            fill.color = Color.Lerp(GameManager.resources.BeginnerColor, GameManager.resources.MasteryColor, testedCard.MasteryScore);
        }

        private void GetNewTestedcard()
        {
            // prioritize the unmastered words
            for (int i = 0; i < GameManager.saveManager.memoryData.Flashcards.Length; i++)
            {
                if (GameManager.saveManager.memoryData.Flashcards[i].MasteryScore < 1f && !DoesRecentIndexesContains(i))
                {
                    //possibleIndexes.Add(i);
                    if (GameManager.saveManager.settingsData.UseWordsAndKanji)
                    {
                        possibleIndexes.Add(i);
                    }
                    else if (GameManager.saveManager.settingsData.UseOnlyWords && !GameManager.saveManager.memoryData.Flashcards[i].IsKanjiCard)
                    {
                        possibleIndexes.Add(i);
                    }
                    else if (GameManager.saveManager.settingsData.UseOnlyKanji && GameManager.saveManager.memoryData.Flashcards[i].IsKanjiCard)
                    {
                        possibleIndexes.Add(i);
                    }
                }
                if (possibleIndexes.Count >= numberOfPossibleWords)
                {
                    break;
                }
            }

            // then prioritize words that were seen a long time ago
            for (int i = 0; i < GameManager.saveManager.memoryData.Flashcards.Length; i++)
            {
                if (!DoesRecentIndexesContains(i))
                {
                    System.DateTime today = System.DateTime.Today;
                    Flashcard testedCard = GameManager.saveManager.memoryData.Flashcards[i];
                    System.DateTime cardDate = new System.DateTime(testedCard.SeenYear, testedCard.SeenMonth, testedCard.SeenDay);
                    if ((today - cardDate).Days > 5)
                    {
                        if (GameManager.saveManager.settingsData.UseWordsAndKanji)
                        {
                            possibleIndexes.Add(i);
                        }
                        else if (GameManager.saveManager.settingsData.UseOnlyWords && !GameManager.saveManager.memoryData.Flashcards[i].IsKanjiCard)
                        {
                            possibleIndexes.Add(i);
                        }
                        else if (GameManager.saveManager.settingsData.UseOnlyKanji && GameManager.saveManager.memoryData.Flashcards[i].IsKanjiCard)
                        {
                            possibleIndexes.Add(i);
                        }
                    }
                }
                if (possibleIndexes.Count >= numberOfPossibleWords)
                {
                    break;
                }
            }

            // fill with mastered words if there are not enough
            int loopSecurity = 0;
            while (possibleIndexes.Count < numberOfPossibleWords)
            {
                loopSecurity++;
                if (loopSecurity > 25) break;
                int v = Random.Range(0, GameManager.saveManager.memoryData.Flashcards.Length);
                if (!possibleIndexes.Contains(v) && !DoesRecentIndexesContains(v))
                {
                    //possibleIndexes.Add(v);
                    if (GameManager.saveManager.settingsData.UseWordsAndKanji)
                    {
                        possibleIndexes.Add(v);
                    }
                    else if (GameManager.saveManager.settingsData.UseOnlyWords && !GameManager.saveManager.memoryData.Flashcards[v].IsKanjiCard)
                    {
                        possibleIndexes.Add(v);
                    }
                    else if (GameManager.saveManager.settingsData.UseOnlyKanji && GameManager.saveManager.memoryData.Flashcards[v].IsKanjiCard)
                    {
                        possibleIndexes.Add(v);
                    }
                }
                else
                {
                    continue;
                }

            }

            string debugPossibles = "";
            for (int i = 0; i < possibleIndexes.Count; i++)
            {
                debugPossibles += (GameManager.saveManager.memoryData.Flashcards[possibleIndexes[i]].KanjiWord + " ");
            }
            Debug.Log(debugPossibles + "are possibles");

            int c = Random.Range(0, possibleIndexes.Count); // get a random index from the possibilities
            testedCard = GameManager.saveManager.memoryData.Flashcards[possibleIndexes[c]]; // get the tested card
            recentIndexes.Add(new IntTracker(possibleIndexes[c], turnsBeforeSameWord + 1)); // add it to the recent cards that were shown
            possibleIndexes.Clear(); // clear the possible indexes to have it fresh for the next turn
            for (int i = 0; i < recentIndexes.Count; i++) // update the trackers ints in the recentIndexes list
            {
                recentIndexes[i].TrackerInt--;
                if (recentIndexes[i].TrackerInt == 0)
                {
                    recentIndexes.RemoveAt(i);
                    i--;
                }
            }

            string debugRecent = "";
            for (int i = 0; i < recentIndexes.Count; i++)
            {
                debugRecent += (GameManager.saveManager.memoryData.Flashcards[recentIndexes[i].SavedInt].KanjiWord + " ");
            }
            Debug.Log(debugRecent + "are recent");



        }

        private bool DoesRecentIndexesContains(int y)
        {
            for (int i = 0; i < recentIndexes.Count; i++)
            {
                if (recentIndexes[i].SavedInt == y)
                {
                    return true;
                }
            }
            return false;
        }

        public void DisplayKAGame()
        {
            GameManager.instance.CallOnChangeMenuTitle("Learn - Kanji attributes");
            choiceGO.SetActive(false);
            gameFKGO.SetActive(false);
            gameKAGO.SetActive(true);
            goBackButton.SetActive(true);
            notEnoughGO.SetActive(false);
            DisplayTestedCardKA();
        }

        public void DisplayFKGame()
        {
            if (!GameManager.saveManager.settingsData.HasFullVersion && GameManager.saveManager.memoryData.Flashcards.Length >= 15)
            {
                GameManager.instance.CallOnChoicePopUpRequested("Full version", "This activity is only accessible with the full version of the app.", "Go to shop", GameManager.instance.CallOnRequestShopOpen, "Continue", null);
                return;
            }
            GameManager.instance.CallOnChangeMenuTitle("Learn - Find kanji");
            choiceGO.SetActive(false);
            gameKAGO.SetActive(false);
            gameFKGO.SetActive(true);
            goBackButton.SetActive(true);
            notEnoughGO.SetActive(false);
            DisplayTestedCardFK();
        }

        public void DisplayChoiceCanvas()
        {
            GameManager.instance.CallOnChangeMenuTitle("Learn");
            gameFKGO.SetActive(false);
            gameKAGO.SetActive(false);
            goBackButton.SetActive(false);
            notEnoughGO.SetActive(false);
            choiceGO.SetActive(true);
            freeVersionReminderGO.SetActive(!GameManager.saveManager.settingsData.HasFullVersion);
        }

        private void UpdateSaveDataAfterAnswer()
        {
            System.DateTime td = System.DateTime.Today;
            testedCard.SeenDay = td.Day;
            testedCard.SeenMonth = td.Month;
            testedCard.SeenYear = td.Year;

            GameManager.saveManager.memoryData.Flashcards[testedCard.Index] = testedCard;
            GameManager.saveManager.SaveMemoryData();
            GameManager.saveManager.statsData.TempStreak = currentWinStreak;
            if (currentWinStreak > GameManager.saveManager.statsData.LongestStreak)
            {
                // add animations and shit
                GameManager.saveManager.statsData.LongestStreak = currentWinStreak;
            }
            GameManager.saveManager.SaveStatsData();
        }
        #endregion

        #region Kanji attributes game functions
        public void RequestDelayedKADisplay()
        {
            StartCoroutine(DelayedKADisplay(0.1f));
        }

        private IEnumerator DelayedKADisplay(float time)
        {
            ClearKAInputAndTexts();
            yield return new WaitForSeconds(time);
            gameKACanvasAnim.SetTrigger("reset");
            DisplayTestedCardKA();
        }

        public void DisplayTestedCardKA() // called everytime the user change to the memory menu, and in an UnityEvent in the animator of the answer text
        {
            GetNewTestedcard();
            answerTextKA.gameObject.SetActive(false);
            hasValidated = false;
            ClearKAInputAndTexts();

            testedKanjiText.text = testedCard.KanjiWord;
            if(testedCard.IsKanjiCard)
            {
                //choose if testing mother tongue, kata or hira
                int c = Random.Range(0, 3);
                if (testedCard.MasteryScore >= 1f) c = Random.Range(0, 2);
                //display clues and kanji and set expected answer
                switch (c)
                {
                    case 0: // testing mother tongue
                        clueOneText.text = testedCard.HiraganaWord;
                        clueTwoText.text = testedCard.KatakanaWord;
                        answerTextKA.text = testedCard.MotherTongueWord;
                        expectedKAAnswer = testedCard.MotherTongueWord;
                        instructionText.text = "translation?";
                        break;
                    case 1: // testing hiragana
                        clueOneText.text = testedCard.KatakanaWord;
                        clueTwoText.text = testedCard.MotherTongueWord;
                        answerTextKA.text = testedCard.HiraganaWord;
                        expectedKAAnswer = testedCard.HiraganaWord;
                        instructionText.text = "kunyomi?";
                        break;
                    case 2: // testing katakana
                        clueOneText.text = testedCard.HiraganaWord;
                        clueTwoText.text = testedCard.MotherTongueWord;
                        answerTextKA.text = testedCard.KatakanaWord;
                        expectedKAAnswer = testedCard.KatakanaWord;
                        instructionText.text = "onyomi?";
                        break;
                }
            }
            else
            {
                //choose if testing mother tongue or kana
                int c = Random.Range(0, 2);
                if (testedCard.MasteryScore >= 1f || testedCard.KanjiWord == testedCard.KanaWord) c = 1;
                //display clue and kanji
                switch (c)
                {
                    case 0: // testing mother tongue
                        clueOneText.text = testedCard.KanaWord;
                        clueTwoText.text = "";
                        answerTextKA.text = testedCard.MotherTongueWord;
                        expectedKAAnswer = testedCard.MotherTongueWord;
                        instructionText.text = "translation?";
                        break;
                    case 1: // testing kana
                        clueOneText.text = testedCard.MotherTongueWord;
                        clueTwoText.text = "";
                        answerTextKA.text = testedCard.KanaWord;
                        expectedKAAnswer = testedCard.KanaWord;
                        instructionText.text = "kana?";
                        break;
                }
            }
            if(testedCard.MasteryScore >= 1f)
            {
                testedKanjiText.color = GameManager.resources.FullMasterColor;
                clueOneText.text = "";
                clueTwoText.text = "";
            }
            else
            {
                testedKanjiText.color = Color.white;
            }

            FillMasterySlider(masterySliderKA, masteryFillerKA);
        }

        public void ValidateAnswerKA() // called on the validate button
        {
            if (hasValidated) return; // this prevent button spamming
            hasValidated = true;

            answerTextKA.gameObject.SetActive(true);
            if(answerField.text == expectedKAAnswer) // right answer
            {
                answerTextKA.color = GameManager.resources.RightColor;
                answerAnimKA.SetTrigger("right");
                testedCard.MasteryScore += 0.1f;
                testedCard.MasteryScore = Mathf.Clamp(testedCard.MasteryScore, 0, 1);
                currentWinStreak++;
                GameManager.instance.CallOnRequestSound(SoundRequest.Right, 0.5f);
            }
            else // wrong answer
            {
                answerTextKA.color = GameManager.resources.WrongColor;
                answerAnimKA.SetTrigger("wrong");
                testedCard.MasteryScore -= 0.1f;
                testedCard.MasteryScore = Mathf.Clamp(testedCard.MasteryScore, 0, 1);
                currentWinStreak = 0;
                GameManager.instance.CallOnRequestSound(SoundRequest.Wrong, 0.5f);
                Vibration.Vibrate();
            }
            Debug.Log(testedCard.MasteryScore);
            FillMasterySlider(masterySliderKA, masteryFillerKA);
            UpdateSaveDataAfterAnswer();
        }

        private void ClearKAInputAndTexts()
        {
            testedKanjiText.text = "";
            clueOneText.text = "";
            clueTwoText.text = "";
            answerTextKA.text = "";
            answerField.text = "";
        }
        #endregion

        #region Find kanji game functions

        public void RequestDelayedFKDisplay()
        {
            StartCoroutine(DelayedFKDisplay(0.1f));
        }

        private IEnumerator DelayedFKDisplay(float time)
        {
            yield return new WaitForSeconds(time);
            gameFKCanvasAnim.SetTrigger("reset");
            DisplayTestedCardFK();
        }

        public void DisplayTestedCardFK()
        {
            GetNewTestedcard();
            SetFKFalseAnswers();
            answerTextFK.gameObject.SetActive(false);
            hasValidated = false;
            testedWordText.text = testedCard.MotherTongueWord;
            answerTextFK.text = testedCard.KanjiWord;

            if (testedCard.MasteryScore >= 1f)
            {
                testedWordText.color = GameManager.resources.FullMasterColor;
            }
            else
            {
                testedWordText.color = Color.white;
            }

            int c = Random.Range(0, 3);
            expectedFKAnswer = c;
            switch(c)
            {
                case 0:
                    answerOneFK.text = testedCard.KanjiWord;
                    clueOneFK.text = testedCard.CommonUseWord();
                    answerTwoFK.text = firstFalseAnswerFK.KanjiWord;
                    clueTwoFK.text = firstFalseAnswerFK.CommonUseWord();
                    answerThreeFK.text = secondFalseAnswerFK.KanjiWord;
                    clueThreeFK.text = secondFalseAnswerFK.CommonUseWord();
                    break;
                case 1:
                    answerTwoFK.text = testedCard.KanjiWord;
                    clueTwoFK.text = testedCard.CommonUseWord();
                    answerOneFK.text = firstFalseAnswerFK.KanjiWord;
                    clueOneFK.text = firstFalseAnswerFK.CommonUseWord();
                    answerThreeFK.text = secondFalseAnswerFK.KanjiWord;
                    clueThreeFK.text = secondFalseAnswerFK.CommonUseWord();
                    break;
                case 2:
                    answerThreeFK.text = testedCard.KanjiWord;
                    clueThreeFK.text = testedCard.CommonUseWord();
                    answerTwoFK.text = firstFalseAnswerFK.KanjiWord;
                    clueTwoFK.text = firstFalseAnswerFK.CommonUseWord();
                    answerOneFK.text = secondFalseAnswerFK.KanjiWord;
                    clueOneFK.text = secondFalseAnswerFK.CommonUseWord();
                    break;
            }

            // fill mastery
            FillMasterySlider(masterySliderFK, masteryFillerFK);
        }

        public void ValidateFKAnswer(int index)
        {
            if (hasValidated) return;
            hasValidated = true;

            answerTextFK.gameObject.SetActive(true);
            if (index == expectedFKAnswer) // right answer
            {
                answerTextFK.color = GameManager.resources.RightColor;
                answerAnimFK.SetTrigger("right");
                testedCard.MasteryScore += 0.1f;
                testedCard.MasteryScore = Mathf.Clamp(testedCard.MasteryScore, 0, 1);
                currentWinStreak++;
                GameManager.instance.CallOnRequestSound(SoundRequest.Right, 0.5f);
            }
            else // wrong answer
            {
                answerTextFK.color = GameManager.resources.WrongColor;
                answerAnimFK.SetTrigger("wrong");
                testedCard.MasteryScore -= 0.1f;
                testedCard.MasteryScore = Mathf.Clamp(testedCard.MasteryScore, 0, 1);
                currentWinStreak = 0;
                GameManager.instance.CallOnRequestSound(SoundRequest.Wrong, 0.5f);
                Vibration.Vibrate();
            }
            FillMasterySlider(masterySliderFK, masteryFillerFK);
            UpdateSaveDataAfterAnswer();

            firstFalseAnswerFK = null;
            secondFalseAnswerFK = null;
        }

        private void SetFKFalseAnswers()
        {
            firstFalseAnswerFK = GetDifferentTranslatedCardFK();
            secondFalseAnswerFK = firstFalseAnswerFK;
            int loopSecurity = 0;
            while (secondFalseAnswerFK == firstFalseAnswerFK)
            {
                secondFalseAnswerFK = GetDifferentTranslatedCardFK();
                loopSecurity++;
                if(loopSecurity > 20)
                {
                    break;
                }
            }
        }

        private Flashcard GetDifferentTranslatedCardFK()
        {
            Flashcard card = null;
            int loopSecurity = 0;
            while (card == null)
            {
                int r = Random.Range(0, GameManager.saveManager.memoryData.Flashcards.Length);
                Flashcard foundCard = GameManager.saveManager.memoryData.Flashcards[r];
                if(foundCard.Index == testedCard.Index)
                {
                    loopSecurity = Mathf.Clamp(loopSecurity - 1, 0, 25);
                    continue;
                }

                loopSecurity++;
                if (loopSecurity > 25)
                {
                    card = foundCard;
                    break;
                }

                if(foundCard.MotherTongueWord == testedCard.MotherTongueWord)
                {
                    continue;
                }

                card = foundCard;
            }

            return card;
        }
        #endregion
    }

    public class IntTracker
    {
        private int savedInt = 0;
        private int trackerInt = 0;

        public IntTracker(int m_savedInt, int m_trackerInt)
        {
            savedInt = m_savedInt;
            trackerInt = m_trackerInt;
        }

        public int SavedInt { get => savedInt; set => savedInt = value; }
        public int TrackerInt { get => trackerInt; set => trackerInt = value; }
    }
}
