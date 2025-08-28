using System.Collections;
using SGGames.Scripts.System;
using TMPro;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

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

        public void AddScore(int startScore, int targetScore)
        {
            m_scoreCountingCanvasGroup.alpha = 1;
            m_scoreText.rectTransform.LeanScale(Vector3.one * 1.2f, 0.1f)
                .setEase(LeanTweenType.easeOutExpo)
                .setLoopPingPong(1);
            StartCoroutine(AnimateNumberIncrease(startScore, targetScore, m_scoreText));
        }
        
        public void AddMultiplier(float startMul, float targetMul)
        {
            m_scoreCountingCanvasGroup.alpha = 1;
            m_multiplierText.rectTransform.LeanScale(Vector3.one * 1.2f, 0.1f)
                .setEase(LeanTweenType.easeOutExpo)
                .setLoopPingPong(1);
            StartCoroutine(AnimateNumberIncrease(startMul, targetMul, m_multiplierText));
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

        private IEnumerator AnimateNumberIncrease(int startValue, int targetValue, TextMeshProUGUI textDisplayer)
        {
            //Break if the value is the same
            if (startValue == targetValue)
            {
                textDisplayer.text = targetValue.ToString();
                yield break;
            }
            float elapsedTime = 0;
            float duration = CardManager.k_ShowScoreTime;
            float start = startValue;
            float target = targetValue;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                int lerpValue = Mathf.RoundToInt(Mathf.Lerp(start, target, t));
                textDisplayer.text = lerpValue.ToString();
                yield return null;
            }
            textDisplayer.text = targetValue.ToString();
        }
        
        private IEnumerator AnimateNumberIncrease(float startValue, float targetValue, TextMeshProUGUI textDisplayer)
        {
            //Break if the value is the same
            if (startValue == targetValue)
            {
                textDisplayer.text = targetValue.ToString("F2");
                yield break;
            }
            float elapsedTime = 0;
            float duration = CardManager.k_ShowScoreTime;
            float start = startValue;
            float target = targetValue;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float lerpValue = Mathf.Lerp(start, target, t);
                textDisplayer.text = lerpValue.ToString("F2");
                yield return null;
            }
            textDisplayer.text = targetValue.ToString("F2");
        }

        public void Reset()
        {
            m_scoreText.text = "0";
            m_multiplierText.text = "1.0";
        }
    }
}
