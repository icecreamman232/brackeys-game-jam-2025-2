using System.Collections.Generic;
using System.Linq;
using SGGames.Scripts.Card;
using UnityEngine;

namespace SGGames.Scripts.System
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private int m_maxHandSize = 5;
        [SerializeField] private Transform[] m_handPositions;
        [SerializeField] private List<CardBehavior> m_cardsInHand;
        [SerializeField] private CardPile m_cardPile;
        [SerializeField] private DiscardPile m_discardPile;
        
        private const float k_MovingToPositionTime = 0.7f;
        private const float k_MovingToPositionDelay = 0.05f;
        private const float k_DiscardMoveTime = 0.3f;

        private void Start()
        {
            m_cardPile.InitializePile();
            DealInitialHand();

        }

        private void DealInitialHand()
        {
            var cardsToDeal = m_cardPile.DrawCards(m_maxHandSize);
            for (int i = 0; i < cardsToDeal.Count; i++)
            {
                var card = cardsToDeal[i];
                AddCardToHand(card, i);
                AnimateCardToHand(card, i, k_MovingToPositionDelay * i);
            }
        }

        private void AddCardToHand(CardBehavior card, int handIndex)
        {
            card.ChangeCardState(CardState.InHand);
            card.SetCardIndex(handIndex);
            card.name = $"{card.name} - Index {handIndex}";
            
            // Ensure the hand list can accommodate the index
            while (m_cardsInHand.Count <= handIndex)
            {
                m_cardsInHand.Add(null);
            }
            m_cardsInHand[handIndex] = card;
        }

        private void RemoveCardFromHand(CardBehavior card)
        {
            int index = card.CardIndex;
            if (index >= 0 && index < m_cardsInHand.Count)
            {
                m_cardsInHand[index] = null;
            }
        }

        public List<int> GetScoresFromSelectedCards()
        {
            var selectedCards = m_cardsInHand.Where(card=>card.IsSelected).ToList();
            var scores = new List<int>();
            foreach (var card in selectedCards)
            {
                scores.Add(card.AttackPts);
            }

            return scores;
        }

        public void DiscardSelectedCards()
        {
            var selectedCards = m_cardsInHand.Where(card=>card.IsSelected).ToList();
            var emptySlot = new List<int>();
            foreach (var card in selectedCards)
            {
                int slot = card.CardIndex;
                emptySlot.Add(slot);
                
                RemoveCardFromHand(card);
                AnimateCardToDiscard(card);
            }
            
            FillEmptySlots(emptySlot);
        }

        private void FillEmptySlots(List<int> emptySlot)
        {
            // Check if we need to reshuffle discard pile
            if (emptySlot.Count > m_cardPile.CardCount)
            {
                ReshuffleDiscardIntoPile();
            }
            
            var newCards = m_cardPile.DrawCards(emptySlot.Count);
            for (int i = 0; i < emptySlot.Count; i++)
            {
                var card = newCards[i];
                var slotIndex = emptySlot[i];
                AddCardToHand(card, slotIndex);
                AnimateCardToHand(card, slotIndex, k_MovingToPositionDelay * i);
            }
        }

        private void ReshuffleDiscardIntoPile()
        {
            var discardedCards = m_discardPile.RemoveAllCards();
            m_cardPile.AddCardsFromDiscard(discardedCards);
        }

        private void AnimateCardToDiscard(CardBehavior card)
        {
            card.transform.LeanMove(m_discardPile.transform.position, k_DiscardMoveTime)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(() => 
                {
                    m_discardPile.AddCardToDiscard(card);
                    m_discardPile.PositionCardAtDiscard(card);
                });

        }
        
        private void AnimateCardToHand(CardBehavior card, int handIndex, float delay)
        {
            card.gameObject.SetActive(true);
            card.transform.LeanMove(m_handPositions[handIndex].position, k_MovingToPositionTime)
                .setEase(LeanTweenType.easeOutCubic)
                .setDelay(delay)
                .setOnComplete(()=> card.transform.position = m_handPositions[handIndex].position);
        }
    }
}
