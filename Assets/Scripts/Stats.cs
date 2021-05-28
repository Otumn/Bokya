using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Otumn.Bokya
{
    public class Stats : Entity
    {
        public Text totalCardsValue;
        public CircleProgressBar masteredCardsBar;
        public Text wordCardsValue;
        public CircleProgressBar masteredWordsBar;
        public Text kanjiCardValue;
        public CircleProgressBar masteredKanjiBar;
        public Text streakValue;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnMenuChanged(MenuType type)
        {
            base.OnMenuChanged(type);
            if(type == MenuType.Stats)
            {
                GameManager.saveManager.UpdateStatsData();
                UpdateValues();
            }
        }

        private void UpdateValues()
        {
            ResetBars();
            StatsData data = GameManager.saveManager.statsData;
            totalCardsValue.text = data.TotalCards.ToString();
            if(data.TotalCards > 0)
            {
                masteredCardsBar.StartFilling((float)data.MasteredCards / (float)data.TotalCards);
            }
            else
            {
                masteredCardsBar.StartFilling(0f);
            }
            wordCardsValue.text = data.WordCards.ToString();
            if(data.WordCards > 0)
            {
                masteredWordsBar.StartFilling((float)data.MasteredWordCards / (float)data.WordCards);
            }
            else
            {
                masteredWordsBar.StartFilling(0f);
            }
            kanjiCardValue.text = data.KanjiCards.ToString();
            if (data.KanjiCards > 0)
            {
                masteredKanjiBar.StartFilling((float)data.MasteredKanjiCards / (float)data.KanjiCards);
            }
            else
            {
                masteredKanjiBar.StartFilling(0f);
            }
            streakValue.text = data.LongestStreak.ToString();
        }

        private void ResetBars()
        {
            masteredCardsBar.ResetValues();
            masteredKanjiBar.ResetValues();
            masteredWordsBar.ResetValues();
        }
    }
}
