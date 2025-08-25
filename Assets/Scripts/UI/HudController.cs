using System;
using System.Collections;
using SGGames.Scripts.System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        private const float k_ShowScoreTime = 1f;
        
        private void Awake()
        {
            m_playButton.OnClickAction = PlayButtonClicked;
            m_discardButton.OnClickAction = DiscardButtonClicked;
        }

        private void PlayButtonClicked()
        {
            Debug.Log("Clicked play button");
            m_cardManager.CountScoreFromSelectedCards(m_scoreDisplayer.AddScore,
                () =>
            {
                InputSystem.actions.Disable();
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
            yield return new WaitForSeconds(k_ShowScoreTime);
            m_scoreDisplayer.HideAll();
            InputSystem.actions.Enable();
        }

        private void DiscardButtonClicked()
        {
            Debug.Log("Clicked discard button");
            m_cardManager.DiscardSelectedCards();
        }
    }
}
