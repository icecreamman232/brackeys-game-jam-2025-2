using System.Collections;
using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using SGGames.Scripts.System;
using UnityEngine;

namespace SGGames.Scripts.UI
{
    public class HudController : MonoBehaviour, IBootStrap
    {
        [SerializeField] private ItemManager m_itemManager;
        [SerializeField] private ScoreManager m_scoreManager;
        [SerializeField] private CardManager m_cardManager;
        [SerializeField] private ButtonController m_playButton;
        [SerializeField] private ButtonController m_discardButton;
        [SerializeField] private ButtonController m_ruleInspectButton;
        [SerializeField] private ButtonController m_closeRuleInspectButton;
        [SerializeField] private CanvasGroup m_ruleInspectCanvasGroup;
        [SerializeField] private RectTransform m_rulesInspectGroup;
        [SerializeField] private ScoreCountingDisplayer m_scoreDisplayer;
        
        private EnergyManager m_energyManager;
        private const float k_ShowFinalScoreTime = 1f;
        
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
            m_scoreManager.CalculateFinalScore();
            m_scoreDisplayer.ShowFinalScore(m_scoreManager.FinalScore);
            if (m_energyManager.IsEnergyDrain)
            {
                m_scoreDisplayer.PlayEnergyDrainAnimation();
                yield return new WaitForSeconds(0.5f);
                var energyInfo = m_energyManager.EnergyInfo;
                m_scoreManager.ApplyEnergyDrain(energyInfo.current, energyInfo.max);
                m_scoreDisplayer.ShowFinalScore(m_scoreManager.FinalScore);
            }
            
            yield return new WaitForSeconds(k_ShowFinalScoreTime);
            m_scoreManager.FinishScoreCounting();
            m_scoreDisplayer.HideAll();
            InputManager.SetActive(true);
            
            m_cardManager.FinishTurn();
        }

        private void DiscardButtonClicked()
        {
            if(!InputManager.IsActivated) return;
            if (!m_cardManager.CanDiscardManually) return;
            m_cardManager.DiscardSelectedCards(true);
        }
        
        private void ShowRuleInspect()
        {
            InputManager.SetActive(false);
            m_rulesInspectGroup.localScale = Vector3.one * 0.8f;
            m_closeRuleInspectButton.OnClickAction = HideRuleInspect;
            m_ruleInspectCanvasGroup.Activate();
            m_rulesInspectGroup.LeanScale(Vector3.one, 0.2f)
                .setEase(LeanTweenType.easeOutCirc);
        }

        private void HideRuleInspect()
        {
            InputManager.SetActive(true);
            m_closeRuleInspectButton.OnClickAction = null;
            m_ruleInspectCanvasGroup.Deactivate();
        }

        public void Install()
        {
            m_playButton.OnClickAction = PlayButtonClicked;
            m_discardButton.OnClickAction = DiscardButtonClicked;
            m_ruleInspectButton.OnClickAction = ShowRuleInspect;
            
            m_cardManager = ServiceLocator.GetService<CardManager>();
            m_itemManager = ServiceLocator.GetService<ItemManager>();
            m_scoreManager = ServiceLocator.GetService<ScoreManager>();
            m_energyManager = ServiceLocator.GetService<EnergyManager>();
        }

        public void Uninstall()
        {
            
        }
    }
}
