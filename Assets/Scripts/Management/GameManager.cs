using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Otumn.Bokya
{
    public class GameManager : MonoBehaviour
    {
        [Header("General resources")]
        public Color fullMasterColor;
        public Color masteryColor;
        public Color beginnerColor;
        public Color selectedColor;
        public Color selectedOppositeColor;
        public Color rightColor;
        public Color wrongColor;
        public Color baseBlue;
        public Color selectedBlue;

        public static GameInstance instance = new GameInstance();
        public static SaveManager saveManager = new SaveManager();
        public static GeneralResources resources = new GeneralResources();

        private void Start()
        {
            Screen.fullScreen = true;
            resources = new GeneralResources(fullMasterColor, masteryColor, beginnerColor, selectedColor, rightColor, wrongColor, selectedOppositeColor, baseBlue, selectedBlue);
            saveManager.InitializeAllData();
        }

        #region Debug methods
        [ContextMenu("Delete data")]
        public void DeleteData()
        {
            saveManager.DeleteAllData();
        }

        [ContextMenu("Print words")]
        public void PrintAllSavedWords()
        {
            saveManager.memoryData.PrintAllWords();
        }

        [ContextMenu("Master all words")]
        public void MasterAllWords()
        {
            saveManager.MasterAllCards();
        }
        #endregion
    }

    public class GameInstance
    {
        private List<Entity> entities = new List<Entity>();

        #region Entity management

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        #endregion

        #region Entity calls

        public void CallOnDatasInitialized()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnDatasInitialized();
            }
        }

        public void CallOnWordEntered()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnWordEntered();
            }
        }

        public void CallOnWordRight()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnWordRight();
            }
        }

        public void CallOnWordWrong()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnWordWrong();
            }
        }

        public void CallOnFlashCardAdded()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnFlashcardAdded();
            }
        }

        public void CallOnFlashcardDeleted()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnFlashcardDeleted();
            }
        }

        public void CallOnMenuChanged(MenuType type)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnMenuChanged(type);
            }
        }

        public void CallOnFocusOnCard(Flashcard focusedCard)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnFocusOnCard(focusedCard);
            }
        }

        public void CallOnChoicePopUpRequested(string title, string info, string positiveText, Action positiveAction, string negativeText, Action negativeAction)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnChoicePopUpRequested(title, info, positiveText, positiveAction, negativeText, negativeAction);
            }
        }

        public void CallOnInfoPopUpRequested(string title, string info, string button)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnInfoPopUpRequested(title, info, button);
            }
        }

        public void CallOnRequestSound(SoundRequest type, float volume)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnRequestSound(type, volume);
            }
        }

        public void CallOnRequestSound(AudioClip clip, float volume)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnRequestSound(clip, volume);
            }
        }

        public void CallOnRequestShopOpen()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnRequestShopOpen();
            }
        }

        public void CallOnFullVersionBought()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnFullVersionBought();
            }
        }

        public void CallOnChangeMenuTitle(string title)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].ChangeMenuTitle(title);
            }
        }

        #endregion

    }

    public class GeneralResources
    {
        private Color fullMasterColor;
        private Color masteryColor;
        private Color beginnerColor;
        private Color selectedColor;
        private Color rightColor;
        private Color wrongColor;
        private Color selectedOppositeColor;
        private Color baseBlue;
        private Color selectedBlue;

        public GeneralResources() { }

        public GeneralResources(Color fullMaster, Color mastery, Color beginner, Color selected, Color right, Color wrong, Color selectedOpposite, Color bBlue, Color sBlue)
        {
            fullMasterColor = fullMaster;
            masteryColor = mastery;
            beginnerColor = beginner;
            selectedColor = selected;
            rightColor = right;
            wrongColor = wrong;
            selectedOppositeColor = selectedOpposite;
            baseBlue = bBlue;
            selectedBlue = sBlue;
        }

        public Color MasteryColor { get => masteryColor;}
        public Color BeginnerColor { get => beginnerColor;}
        public Color SelectedColor { get => selectedColor;}
        public Color FullMasterColor { get => fullMasterColor;}
        public Color RightColor { get => rightColor;}
        public Color WrongColor { get => wrongColor;}
        public Color SelectedOppositeColor { get => selectedOppositeColor;}
        public Color BaseBlue { get => baseBlue; set => baseBlue = value; }
        public Color SelectedBlue { get => selectedBlue; set => selectedBlue = value; }
    }
}
