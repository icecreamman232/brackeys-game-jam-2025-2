using System.Collections;
using SGGames.Scripts.Managers;
using SGGames.Scripts.System;
using UnityEngine;

namespace SGGames.Scripts.UI
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private ItemManager m_itemManager;
        [SerializeField] private ScoreManager m_scoreManager;
        [SerializeField] private CardManager m_cardManager;
        [SerializeField] private ButtonController m_playButton;
        [SerializeField] private ButtonController m_discardButton;
        [SerializeField] private ScoreCountingDisplayer m_scoreDisplayer;
        
        private const float k_ShowFinalScoreTime = 1f;
        
        private void Awake()
        {
            m_playButton.OnClickAction = PlayButtonClicked;
            m_discardButton.OnClickAction = DiscardButtonClicked;
        }

        private void PlayButtonClicked()
        {
            if(!InputManager.IsActivated) return;
            
            InputManager.SetActive(false);
            m_scoreDisplayer.Reset();
            m_cardManager.CountScoreFromSelectedCards(m_scoreDisplayer.AddScore,
                () =>
                {
                    m_itemManager.TriggerItem(m_scoreDisplayer.AddMultiplier, ()=>
                    {
                        StartCoroutine(OnProcessShowingFinalScore());
                    });
            });
        }

        private IEnumerator OnProcessShowingFinalScore()
        {
            m_scoreDisplayer.HideScoreCounting();
            m_scoreManager.FinishScoreCounting();
            m_scoreDisplayer.ShowFinalScore(m_scoreManager.FinalScore);
            yield return new WaitForSeconds(k_ShowFinalScoreTime);
            m_scoreDisplayer.HideAll();
            InputManager.SetActive(true);
            
            m_cardManager.FinishTurn();
        }

        private void DiscardButtonClicked()
        {
            if(!InputManager.IsActivated) return;
            m_cardManager.DiscardSelectedCards();
        }
    }
}
