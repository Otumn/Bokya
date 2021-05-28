using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class WordList : Entity
    {
        public RectTransform wordsParent;
        public Vector3 parentInitialPosition;
        public float yPositionDecrement = 60f;
        public GameObject wordPrefab;
        public float timeBeforeWordAppear = 0.1f;
        public InputField searchField;

        [Header("List movement")]
        public float movementSpeed = 250f;
        public AnimationCurve listSlideCurve;
        public float curveSpeed = 0.01f;
        [Header("Filters")]
        public GameObject filtersGO;
        public Animator filtersAnim;
        public Toggle unmasteredToggle;
        public Toggle masteredToggle;
        public Toggle wordToggle;
        public Toggle kanjiToggle;

        private List<WordPrefab> wordPrefabs = new List<WordPrefab>();

        private Vector3 lastTouchPos = new Vector3();
        private float touchesYDiff;
        private float curveIncrementer;
        private bool touchHeld = false;
        private bool isListSliding = false;

        protected override void Update()
        {
            base.Update();
            CheckForInputs();
            SlidingManagement();
        }

        public override void OnMenuChanged(MenuType type)
        {
            base.OnMenuChanged(type);
            if(type == MenuType.WordList)
            {
                DisplayWords(FilterOutCardsArray(GameManager.saveManager.memoryData.Flashcards));
                searchField.text = "";
            }
        }

        public override void OnFlashcardDeleted()
        {
            base.OnFlashcardDeleted();
            DisplayWords(GameManager.saveManager.memoryData.Flashcards);
        }

        private void DisplayWords(Flashcard[] displayedArray)
        {
            for (int i = 0; i < wordPrefabs.Count; i++)
            {
                Destroy(wordPrefabs[i].gameObject);
            }
            wordPrefabs.Clear();
            for (int i = 0; i < displayedArray.Length; i++)
            {
                WordPrefab addedWord = GameObject.Instantiate(wordPrefab, wordsParent).GetComponent<WordPrefab>();
                addedWord.Initialize(displayedArray[i].KanjiWord, displayedArray[i].MotherTongueWord, i * timeBeforeWordAppear, displayedArray[i].Index, displayedArray[i].MasteryScore);
                addedWord.transform.localPosition = new Vector3(0, -i * yPositionDecrement, 0);
                wordPrefabs.Add(addedWord);
            }
            wordsParent.anchoredPosition = new Vector2(0, Mathf.Clamp(wordsParent.anchoredPosition.y, parentInitialPosition.y, wordPrefabs.Count * yPositionDecrement));
        }

        private void RefreshWords(List<WordPrefab> displayedWords)
        {
            for (int i = 0; i < displayedWords.Count; i++)
            {
                displayedWords[i].ShortCutToAnim(timeBeforeWordAppear * i);
            }
        }

        public void OnSearchBarEdited(string input)
        {
            wordsParent.anchoredPosition = parentInitialPosition;
            Flashcard testedCard = new Flashcard();
            List<Flashcard> searchedCards = new List<Flashcard>();
            for (int i = 0; i < GameManager.saveManager.memoryData.Flashcards.Length; i++)
            {
                testedCard = GameManager.saveManager.memoryData.Flashcards[i];
                if(testedCard.KanjiWord.Contains(input) || testedCard.KanaWord.Contains(input) || testedCard.HiraganaWord.Contains(input) || testedCard.KatakanaWord.Contains(input) || testedCard.MotherTongueWord.Contains(input))
                {
                    searchedCards.Add(testedCard);
                }
            }
            DisplayWords(FilterOutCardsArray(searchedCards.ToArray()));
        }

        #region Movement
        private void CheckForInputs()
        {
            if (Input.touchCount > 0)
            {
                if (!touchHeld)
                {
                    touchHeld = true;
                    lastTouchPos = Input.GetTouch(0).position;
                }
                MoveList(Input.GetTouch(0).position);
            }
            else
            {
                if (touchHeld)
                {
                    touchHeld = false;
                    ReleaseListMovement();
                }
            }
            
#if UNITY_EDITOR
            // editor
            if (Input.GetMouseButtonDown(0))
            {
                lastTouchPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                MoveList(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                ReleaseListMovement();
            }
#endif
        }

        private void MoveList(Vector3 touchPos)
        {
            touchesYDiff = (touchPos.y - lastTouchPos.y) / Screen.height;
            wordsParent.anchoredPosition += new Vector2(0, touchesYDiff * movementSpeed * Time.deltaTime);
            wordsParent.anchoredPosition = new Vector2(0, Mathf.Clamp(wordsParent.anchoredPosition.y, parentInitialPosition.y, wordPrefabs.Count * yPositionDecrement));
            lastTouchPos = touchPos;
        }

        private void ReleaseListMovement()
        {
            isListSliding = true;
            curveIncrementer = 0;
        }

        private void SlidingManagement()
        {
            if(isListSliding)
            {
                wordsParent.anchoredPosition += new Vector2(0, touchesYDiff * listSlideCurve.Evaluate(curveIncrementer) * movementSpeed * Time.deltaTime);
                wordsParent.anchoredPosition = new Vector2(0, Mathf.Clamp(wordsParent.anchoredPosition.y, parentInitialPosition.y, wordPrefabs.Count * yPositionDecrement));
                curveIncrementer += curveSpeed;
                if (curveIncrementer >= 1) isListSliding = false;
            }
        }
        #endregion

        #region Filters
        public void OpenFilters()
        {
            filtersGO.SetActive(true);
            filtersAnim.SetTrigger("appear");
            GameManager.instance.CallOnRequestSound(SoundRequest.Click, 0.5f);
        }

        public void CloseFilters()
        {
            filtersAnim.SetTrigger("disappear");
            DisplayWords(FilterOutCardsArray(GameManager.saveManager.memoryData.Flashcards));
        }

        public void DisableFilters()
        {
            filtersGO.SetActive(false);
        }

        public void UpdateMasteryFilters(int index) // 0 = unmastered, 1 = mastered
        {
            if(index == 0)
            {
                if(!masteredToggle.isOn && !unmasteredToggle.isOn)
                {
                    unmasteredToggle.isOn = true;
                }
            }
            else if(index == 1)
            {
                if (!masteredToggle.isOn && !unmasteredToggle.isOn)
                {
                    masteredToggle.isOn = true;
                }
            }
        }

        public void UpdateTypeFilters(int index) // 0 = words, 1 = kanji
        {
            if(index == 0)
            {
                if (!kanjiToggle.isOn && !wordToggle.isOn)
                {
                    wordToggle.isOn = true;
                }
            }
            else if(index == 1)
            {
                if (!kanjiToggle.isOn && !wordToggle.isOn)
                {
                    kanjiToggle.isOn = true;
                }
            }
        }

        private Flashcard[] FilterOutCardsArray(Flashcard[] array)
        {
            List<Flashcard> filtered = new List<Flashcard>();
            for (int i = 0; i < array.Length; i++)
            {
                if (!masteredToggle.isOn && array[i].MasteryScore >= 1f) continue;
                if (!unmasteredToggle.isOn && array[i].MasteryScore <= 0f) continue;
                if (!wordToggle.isOn && !array[i].IsKanjiCard) continue;
                if (!kanjiToggle.isOn && array[i].IsKanjiCard) continue;
                filtered.Add(array[i]);
            }

            return filtered.ToArray();
        }

        #endregion
    }
}
