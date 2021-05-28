using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Otumn.Bokya
{
    public class SaveManager
    {
        public MemoryData memoryData;
        public SettingsData settingsData;
        public StatsData statsData;

        private string memoryDataPath;
        private string settingsDataPath;
        private string statsDataPath;

        public void InitializeAllData()
        {
            memoryDataPath = Path.Combine(Application.persistentDataPath, "MemoryData.otm");
            memoryData = new MemoryData();
            LoadMemoryData();
            settingsDataPath = Path.Combine(Application.persistentDataPath, "SettingsData.otm");
            settingsData = new SettingsData();
            LoadSettingsData();
            statsDataPath = Path.Combine(Application.persistentDataPath, "StatsData.otm");
            statsData = new StatsData();
            LoadStatsData();
            UpdateStatsData();
            GameManager.instance.CallOnDatasInitialized();
        }

        #region Memory save functions
        public void SaveMemoryData()
        {
            string jsonString = JsonUtility.ToJson(memoryData);
            using (StreamWriter streamWriter = File.CreateText(memoryDataPath))
            {
                streamWriter.Write(jsonString);
                Debug.Log("Memory file saved!");
            }
        }

        public void LoadMemoryData()
        {
            if (!File.Exists(memoryDataPath)) // no file found
            {
                // save
                Debug.Log("No saved memory file found, creating one...");
                SaveMemoryData();
                LoadMemoryData();
            }
            else // load file
            {
                Debug.Log("Memory file found");
                using (StreamReader streamReader = File.OpenText(memoryDataPath))
                {
                    string jsonString = streamReader.ReadToEnd();
                    memoryData = JsonUtility.FromJson<MemoryData>(jsonString);
                }
            }
        }
        #endregion

        #region Settings save functions
        public void SaveSettingsData()
        {
            string jsonString = JsonUtility.ToJson(settingsData);
            using (StreamWriter streamWriter = File.CreateText(settingsDataPath))
            {
                streamWriter.Write(jsonString);
                Debug.Log("Settings file saved!");
            }
        }

        public void LoadSettingsData()
        {
            if (!File.Exists(settingsDataPath)) // no file found
            {
                // save
                Debug.Log("No saved settings file found, creating one...");
                SaveSettingsData();
                LoadSettingsData();
            }
            else // load file
            {
                Debug.Log("Settings file found");
                using (StreamReader streamReader = File.OpenText(settingsDataPath))
                {
                    string jsonString = streamReader.ReadToEnd();
                    settingsData = JsonUtility.FromJson<SettingsData>(jsonString);
                }
            }
        }
        #endregion

        #region Stats save functions
        public void SaveStatsData()
        {
            string jsonString = JsonUtility.ToJson(statsData);
            using (StreamWriter streamWriter = File.CreateText(statsDataPath))
            {
                streamWriter.Write(jsonString);
                Debug.Log("Stats file saved!");
            }
        }

        public void LoadStatsData()
        {
            if (!File.Exists(statsDataPath)) // no file found
            {
                // save
                Debug.Log("No saved stats file found, creating one...");
                SaveStatsData();
                LoadStatsData();
            }
            else // load file
            {
                Debug.Log("Stats file found");
                using (StreamReader streamReader = File.OpenText(statsDataPath))
                {
                    string jsonString = streamReader.ReadToEnd();
                    statsData = JsonUtility.FromJson<StatsData>(jsonString);
                }
            }
        }

        public void UpdateStatsData()
        {
            Flashcard[] fcs = memoryData.Flashcards;
            statsData.TotalCards = fcs.Length;
            int masteredCards = 0;
            int wordCards = 0;
            int masteredWordCards = 0;
            int kanjiCards = 0;
            int masteredKanjiCards = 0;
            for (int i = 0; i < fcs.Length; i++)
            {
                if(fcs[i].IsKanjiCard)
                {
                    kanjiCards++;
                    if(fcs[i].MasteryScore >= 1)
                    {
                        masteredCards++;
                        masteredKanjiCards++;
                    }
                }
                else
                {
                    wordCards++;
                    if (fcs[i].MasteryScore >= 1)
                    {
                        masteredCards++;
                        masteredWordCards++;
                    }
                }
            }
            statsData.MasteredCards = masteredCards;
            statsData.WordCards = wordCards;
            statsData.MasteredWordCards = masteredWordCards;
            statsData.KanjiCards = kanjiCards;
            statsData.MasteredKanjiCards = masteredKanjiCards;
            SaveStatsData();
        }
        #endregion

        #region Delete functions
        public void DeleteAllData()
        {
            memoryDataPath = Path.Combine(Application.persistentDataPath, "MemoryData.otm");
            if (File.Exists(memoryDataPath))
            {
                File.Delete(memoryDataPath);
                Debug.Log("Memory file deleted");
            }
            else
            {
                Debug.Log("No memory save detected, can't delete.");
            }
            settingsDataPath = Path.Combine(Application.persistentDataPath, "SettingsData.otm");
            if (File.Exists(settingsDataPath))
            {
                File.Delete(settingsDataPath);
                Debug.Log("Settings file deleted");
            }
            else
            {
                Debug.Log("No settings save detected, can't delete.");
            }
            statsDataPath = Path.Combine(Application.persistentDataPath, "StatsData.otm");
            if (File.Exists(statsDataPath))
            {
                File.Delete(statsDataPath);
                Debug.Log("Stats file deleted");
            }
            else
            {
                Debug.Log("No stats save detected, can't delete.");
            }
        }

        public void ResetApp()
        {
            memoryDataPath = Path.Combine(Application.persistentDataPath, "MemoryData.otm");
            if (File.Exists(memoryDataPath))
            {
                File.Delete(memoryDataPath);
                Debug.Log("Memory file deleted");
            }
            else
            {
                Debug.Log("No memory save detected, can't delete.");
            }
            settingsDataPath = Path.Combine(Application.persistentDataPath, "SettingsData.otm");
            if (File.Exists(settingsDataPath))
            {
                /*File.Delete(settingsDataPath);
                Debug.Log("Settings file deleted");*/

                LoadSettingsData();
                SettingsData tempData = new SettingsData();
                tempData.HasFullVersion = settingsData.HasFullVersion;
                SaveSettingsData();
            }
            else
            {
                Debug.Log("No settings save detected, can't delete.");
            }
            statsDataPath = Path.Combine(Application.persistentDataPath, "StatsData.otm");
            if (File.Exists(statsDataPath))
            {
                File.Delete(statsDataPath);
                Debug.Log("Stats file deleted");
            }
            else
            {
                Debug.Log("No stats save detected, can't delete.");
            }
            Application.Quit();
        }

        public void DeleteFlashcard(int index)
        {
            List<Flashcard> fcs = new List<Flashcard>(memoryData.Flashcards);
            if (index > fcs.Count - 1) return;
            fcs.RemoveAt(index);
            for (int i = 0; i < fcs.Count; i++)
            {
                fcs[i].Index = i;
            }
            memoryData.Flashcards = fcs.ToArray();
            SaveMemoryData();
        }

        public void DeleteAllFlashcards()
        {
            memoryData = new MemoryData();
            SaveMemoryData();
            UpdateStatsData();
        }
        #endregion

        #region Debug functions
        public void MasterAllCards()
        {
            for (int i = 0; i < GameManager.saveManager.memoryData.Flashcards.Length; i++)
            {
                GameManager.saveManager.memoryData.Flashcards[i].MasteryScore = 1f;
            }
            GameManager.saveManager.SaveMemoryData();
        }
        #endregion
    }

    [Serializable]
    public class MemoryData
    {
        [SerializeField] private Flashcard[] flashcards = new Flashcard[0] { };

        public Flashcard[] Flashcards { get => flashcards; set => flashcards = value; }

        public void PrintAllWords() // debug method
        {
            for (int i = 0; i < Flashcards.Length; i++)
            {
                Debug.Log(Flashcards[i].Index + " : " + Flashcards[i].KanjiWord);
            }
        }

    }

    [Serializable]
    public class Flashcard
    {
        [SerializeField] private int index = 0;
        [SerializeField] private bool isKanjiCard = false;
        [SerializeField] private string kanjiWord = "";
        [SerializeField] private string motherTongueWord = "";
        [SerializeField] private float masteryScore = 0;
        [SerializeField] private int seenDay = 0;
        [SerializeField] private int seenMonth = 0;
        [SerializeField] private int seenYear = 0;

        #region Non Kanji card variables
        [SerializeField] private string kanaWord = "";
        #endregion

        #region Kanji card variables
        [SerializeField] private string hiraganaWord = "";
        [SerializeField] private string katakanaWord = "";
        #endregion

        #region Encapsulation
        public int Index { get => index; set => index = value; }
        public bool IsKanjiCard { get => isKanjiCard; set => isKanjiCard = value; }
        public string KanjiWord { get => kanjiWord; set => kanjiWord = value; }
        public string MotherTongueWord { get => motherTongueWord; set => motherTongueWord = value; }
        public string KanaWord { get => kanaWord; set => kanaWord = value; }
        public string HiraganaWord { get => hiraganaWord; set => hiraganaWord = value; }
        public string KatakanaWord { get => katakanaWord; set => katakanaWord = value; }
        public float MasteryScore { get => masteryScore; set => masteryScore = value; }
        public int SeenDay { get => seenDay; set => seenDay = value; }
        public int SeenMonth { get => seenMonth; set => seenMonth = value; }
        public int SeenYear { get => seenYear; set => seenYear = value; }
        #endregion

        #region Utility
        public string CommonUseWord()
        {
            if (isKanjiCard) return hiraganaWord;
            else return kanaWord;
        }
        #endregion
    }

    [Serializable]
    public class SettingsData
    {
        [SerializeField] private bool tutorialDone = false;
        [SerializeField] private float volume = 0.5f;
        [SerializeField] private bool enableVibrations = true;
        [SerializeField] private bool useWordsAndKanji = true;
        [SerializeField] private bool useOnlyWords = false;
        [SerializeField] private bool useOnlyKanji = false;
        [SerializeField] private bool hasFullVersion = false;

        public float Volume { get => volume; set => volume = value; }
        public bool EnableVibrations { get => enableVibrations; set => enableVibrations = value; }
        public bool UseWordsAndKanji { get => useWordsAndKanji; set => useWordsAndKanji = value; }
        public bool UseOnlyWords { get => useOnlyWords; set => useOnlyWords = value; }
        public bool UseOnlyKanji { get => useOnlyKanji; set => useOnlyKanji = value; }
        public bool TutorialDone { get => tutorialDone; set => tutorialDone = value; }
        public bool HasFullVersion { get => hasFullVersion; set => hasFullVersion = value; }

        public SettingsData()
        {

        }

        public SettingsData(float m_volume, bool m_vibrations, bool m_useBoth, bool m_useWords, bool m_useKanji, bool fullVersion)
        {
            volume = m_volume;
            enableVibrations = m_vibrations;
            useWordsAndKanji = m_useBoth;
            useOnlyWords = m_useWords;
            useOnlyKanji = m_useKanji;
            hasFullVersion = fullVersion;
        }
    }

    [Serializable]
    public class StatsData
    {
        [SerializeField] private int totalCards = 0;
        [SerializeField] private int masteredCards = 0;
        [SerializeField] private int wordCards = 0;
        [SerializeField] private int masteredWordCards = 0;
        [SerializeField] private int kanjiCards = 0;
        [SerializeField] private int masteredKanjiCards = 0;
        [SerializeField] private int longestStreak = 0;
        [SerializeField] private int tempStreak = 0;

        public int TotalCards { get => totalCards; set => totalCards = value; }
        public int MasteredCards { get => masteredCards; set => masteredCards = value; }
        public int WordCards { get => wordCards; set => wordCards = value; }
        public int MasteredWordCards { get => masteredWordCards; set => masteredWordCards = value; }
        public int KanjiCards { get => kanjiCards; set => kanjiCards = value; }
        public int MasteredKanjiCards { get => masteredKanjiCards; set => masteredKanjiCards = value; }
        public int LongestStreak { get => longestStreak; set => longestStreak = value; }
        public int TempStreak { get => tempStreak; set => tempStreak = value; }
    }
}
