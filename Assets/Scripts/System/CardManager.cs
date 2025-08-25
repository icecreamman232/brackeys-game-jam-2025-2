using System.Collections.Generic;
using SGGames.Scripts.Card;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SGGames.Scripts.System
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private List<CardBehavior> m_cardsInPile;
        [SerializeField] private List<CardBehavior> m_cardsInHand;
        [SerializeField] private List<CardBehavior> m_cardsInDiscard;
        [SerializeField] private CardPile m_cardPile;
        [SerializeField] private DiscardPile m_discardPile;
        [SerializeField] private Transform m_pilePivot;
        [SerializeField] private Transform m_handPivot;
        [SerializeField] private Transform m_discardPivot;
        
        public List<CardBehavior> CardsInPile => m_cardsInPile;
        public List<CardBehavior> CardsInHand => m_cardsInHand;
        public List<CardBehavior> CardsInDiscard => m_cardsInDiscard;
        
        public void AddNewCardToPile(CardBehavior card)
        {
            card.ChangeCardState(CardState.InPile);
            m_cardsInPile.Add(card);
            card.transform.SetParent(m_pilePivot, worldPositionStays:true);
            card.transform.localPosition = Vector3.zero;
        }
        
        public void AddCardToHand(CardBehavior card)
        {
            m_cardsInPile.Remove(card);
            card.ChangeCardState(CardState.InHand);
            m_cardsInHand.Add(card);
            card.transform.SetParent(m_handPivot, worldPositionStays:true);
            card.transform.localPosition = Vector3.zero;
        }

        public void DiscardSelectedCard()
        {
            var discardIndexList = new List<int>();
            var cardsToDiscard = new List<CardBehavior>();
            
            // First, collect all cards to discard
            foreach (var card in m_cardsInHand)
            {
                //Only discard selected cards in hand
                if (card.IsSelected && card.CardState == CardState.InHand)
                {
                    cardsToDiscard.Add(card);
                    discardIndexList.Add(card.CardIndex);
                }
            }
            
            // Then process the discarding
            foreach (var card in cardsToDiscard)
            {
                card.transform.LeanMove(m_discardPile.transform.position, 0.5f)
                    .setEase(LeanTweenType.easeOutCirc)
                    .setOnComplete(()=> OnFinishMoveToDiscardPile(card));
                
                //Reset card index, since they are no longer in hand
                card.SetCardIndex(-1);
                
                m_cardsInHand.Remove(card);
                m_cardsInDiscard.Add(card);
                card.ChangeCardState(CardState.InDiscard);
                card.transform.SetParent(m_discardPivot, worldPositionStays:true);
                card.transform.localPosition = Vector3.zero;
            }
            
            //Deal new cards into empty slot in hand
            m_cardPile.DealCardIntoIndex(discardIndexList);
        }
        
        public void BringCardFromDiscardBack()
        {
            foreach (var card in m_cardsInDiscard)
            {
                card.ChangeCardState(CardState.InPile);
                m_cardsInPile.Add(card);
                card.transform.position = m_cardPile.transform.position;
                card.transform.SetParent(m_pilePivot, worldPositionStays:true);
                card.transform.localPosition = Vector3.zero;
                card.gameObject.SetActive(false);
            }
            m_cardsInDiscard.Clear();
        }
        
        private void OnFinishMoveToDiscardPile(CardBehavior card)
        {
            card.transform.position = m_discardPile.transform.position;
            card.gameObject.SetActive(false);
        }
    }
}
