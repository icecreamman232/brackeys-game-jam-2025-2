using SGGames.Scripts.Card;
using SGGames.Scripts.Core;
using SGGames.Scripts.Data;
using UnityEngine;


namespace SGGames.Scripts.UI
{
    public class PickNewCardUIState : WinGameUIState
    {
        private CanvasGroup m_canvasGroup;
        private CardPile m_cardPile;
        private CardData m_selectedCard;
        private CardSelectionUI[] m_cardSelectionUIList;
        public CardData SelectedCard => m_selectedCard;
        
        public PickNewCardUIState(WinGameUIStateType stateType, CanvasGroup canvasGroup, CardPile cardPile, CardSelectionUI[] cardSelectionUIList) 
            : base(stateType)
        {
            m_canvasGroup = canvasGroup;
            m_cardPile = cardPile;
            m_cardSelectionUIList = cardSelectionUIList;
            foreach (var cardSelection in m_cardSelectionUIList)
            {
                cardSelection.OnClickAction = OnClickCardSelection;
            }
        }
        
        public override void Initialize()
        {
            
        }

        public override void EnterState()
        {
            var cardSelections = m_cardPile.GetCardsData(3);
            for (int i = 0; i < cardSelections.Count; i++)
            {
                var cardData  = cardSelections[i];
                m_cardSelectionUIList[i].SetCard(cardData);
            }
            
            m_canvasGroup.Activate();
            
            for (int i = 0; i < cardSelections.Count; i++)
            {
                ((RectTransform)m_cardSelectionUIList[i].transform).LeanMoveLocalY(0, 0.2f)
                    .setEase(LeanTweenType.easeOutCirc)
                    .setDelay(i * 0.15f);
            }
        }

        public override void ExitState()
        {
            foreach (var cardSelection in m_cardSelectionUIList)
            {
                cardSelection.SetSelect(false);
            }
            
            m_canvasGroup.Deactivate();
        }

        public override void Hide()
        {
            foreach (var cardSelection in m_cardSelectionUIList)
            {
                cardSelection.SetSelect(false);
            }
            m_canvasGroup.Deactivate();
        }

        private void OnClickCardSelection(CardData cardData)
        {
            foreach (var cardSelection in m_cardSelectionUIList)
            {
                if (cardSelection.CardData.Name == cardData.Name)
                {
                    cardSelection.SetSelect(true);
                    m_selectedCard = cardData;
                }
                else
                {
                    cardSelection.SetSelect(false);
                }
            }
        }
    }
}
