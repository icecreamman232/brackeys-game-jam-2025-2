using System;
using TMPro;
using UnityEngine;

namespace SGGames.Scripts.UI
{
    public class ScoreCountingDisplayer : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TextMeshProUGUI m_scoreText;
        [SerializeField] private TextMeshProUGUI m_multiplierText;
        
        private void Start()
        {
            m_canvasGroup.alpha = 0;
        }

        public void AddScore(int score)
        {
            m_canvasGroup.alpha = 1;
            m_scoreText.text = score.ToString();
        }
        
        public void AddMultiplier(int multiplier)
        {
            m_canvasGroup.alpha = 1;
            m_multiplierText.text = multiplier.ToString();
        }

        public void HideDisplayer()
        {
            m_canvasGroup.alpha = 0;
        }
    }
}
