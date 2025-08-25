using System;
using SGGames.Scripts.System;
using UnityEngine;

namespace SGGames.Scripts.UI
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private ScoreManager m_scoreManager;
        [SerializeField] private CardManager m_cardManager;
        [SerializeField] private ButtonController m_playButton;
        [SerializeField] private ButtonController m_discardButton;
        [SerializeField] private ScoreCountingDisplayer m_scoreDisplayer;

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
                m_scoreDisplayer.HideDisplayer();
            });
        }

        private void DiscardButtonClicked()
        {
            Debug.Log("Clicked discard button");
            m_cardManager.DiscardSelectedCards();
        }
    }
}
