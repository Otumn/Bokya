using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class UIManager : Entity
    {
        public GameObject wordListGO;
        public GameObject addWordGO;
        public GameObject memoryGO;
        public GameObject statsGO;
        public GameObject settingsGO;
        public GameObject tutorialGO;
        public Text menuTitle;

        public override void OnDatasInitialized()
        {
            base.OnDatasInitialized();
            if(!GameManager.saveManager.settingsData.TutorialDone)
            {
                GameManager.instance.CallOnMenuChanged(MenuType.Tutorial);
            }
            else
            {
                GameManager.instance.CallOnMenuChanged(MenuType.Memory);
            }
        }

        public override void OnMenuChanged(MenuType type)
        {
            base.OnMenuChanged(type);
            switch(type)
            {
                case MenuType.WordList:
                    wordListGO.SetActive(true);
                    addWordGO.SetActive(false);
                    memoryGO.SetActive(false);
                    statsGO.SetActive(false);
                    settingsGO.SetActive(false);
                    tutorialGO.SetActive(false);
                    menuTitle.text = "Card list";
                    break;

                case MenuType.AddWord:
                    wordListGO.SetActive(false);
                    addWordGO.SetActive(true);
                    memoryGO.SetActive(false);
                    statsGO.SetActive(false);
                    settingsGO.SetActive(false);
                    tutorialGO.SetActive(false);
                    menuTitle.text = "Add a card";
                    break;

                case MenuType.Memory:
                    wordListGO.SetActive(false);
                    addWordGO.SetActive(false);
                    memoryGO.SetActive(true);
                    statsGO.SetActive(false);
                    settingsGO.SetActive(false);
                    tutorialGO.SetActive(false);
                    menuTitle.text = "Learn";
                    break;

                case MenuType.Stats:
                    wordListGO.SetActive(false);
                    addWordGO.SetActive(false);
                    memoryGO.SetActive(false);
                    statsGO.SetActive(true);
                    settingsGO.SetActive(false);
                    tutorialGO.SetActive(false);
                    menuTitle.text = "Stats";
                    break;

                case MenuType.Settings:
                    wordListGO.SetActive(false);
                    addWordGO.SetActive(false);
                    memoryGO.SetActive(false);
                    statsGO.SetActive(false);
                    settingsGO.SetActive(true);
                    tutorialGO.SetActive(false);
                    menuTitle.text = "Settings";
                    break;
                case MenuType.Tutorial:
                    wordListGO.SetActive(false);
                    addWordGO.SetActive(false);
                    memoryGO.SetActive(false);
                    statsGO.SetActive(false);
                    settingsGO.SetActive(false);
                    tutorialGO.SetActive(true);
                    menuTitle.text = "Tutorial";
                    break;
            }
        }

        public override void ChangeMenuTitle(string title)
        {
            base.ChangeMenuTitle(title);
            menuTitle.text = title;
        }
    }

    public enum MenuType
    {
        WordList,
        AddWord,
        Memory,
        Stats,
        Settings,
        Tutorial,
    };
}
