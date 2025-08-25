using TMPro;
using UnityEngine;

namespace SGGames.Scripts.UI
{
    public class ScoreCountingDisplayer : MonoBehaviour
    {
        [Header("Counting")]
        [SerializeField] private CanvasGroup m_scoreCountingCanvasGroup;
        [SerializeField] private TextMeshProUGUI m_scoreText;
        [SerializeField] private TextMeshProUGUI m_multiplierText;
        [Header("Final Score")]
        [SerializeField] private CanvasGroup m_finalScoreCanvasGroup;
        [SerializeField] private TextMeshProUGUI m_finalScoreText;
        
        private void Start()
        {
            m_scoreCountingCanvasGroup.alpha = 0;
            m_finalScoreCanvasGroup.alpha = 0;
        }

        public void AddScore(int score)
        {
            m_scoreCountingCanvasGroup.alpha = 1;
            m_scoreText.text = score.ToString();
        }
        
        public void AddMultiplier(float multiplier)
        {
            m_scoreCountingCanvasGroup.alpha = 1;
            m_multiplierText.text = multiplier.ToString("F2");
        }

        public void ShowFinalScore(int finalScore)
        {
            m_finalScoreCanvasGroup.alpha = 1;
            m_finalScoreText.text = finalScore.ToString();
        }

        public void HideScoreCounting()
        {
            m_scoreCountingCanvasGroup.alpha = 0;
        }

        public void HideAll()
        {
            m_scoreCountingCanvasGroup.alpha = 0;
            m_finalScoreCanvasGroup.alpha = 0;
        }
    }
}
