using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SGGames.Scripts.Card;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SGGames.Scripts.System
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private ScoreManager m_scoreManager;
        [SerializeField] private int m_maxHandSize = 5;
        [SerializeField] private Transform[] m_handPositions;
        [SerializeField] private List<CardBehavior> m_cardsInHand;
        [SerializeField] private CardPile m_cardPile;
        [SerializeField] private DiscardPile m_discardPile;
        
        private const float k_MovingToPositionTime = 0.7f;
        private const float k_MovingToPositionDelay = 0.05f;
        private const float k_DiscardMoveTime = 0.3f;
        private const float k_ShowScoreTime = 0.3f;

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
            card.SetName();
            card.IsOverlappedOnCard = IsCardOverlapping;
            card.SwapCardsAction = SwapCard;
            
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

        public void CountScoreFromSelectedCards(Action<int> addingScoreToUIAction, Action onFinish)
        {
            StartCoroutine(OnCountingScore(addingScoreToUIAction, onFinish));
        }
        
        private IEnumerator OnCountingScore(Action<int> addingScoreToUIAction, Action onFinish)
        {
            InputSystem.actions.Disable();
            
            var selectedCards = m_cardsInHand.Where(card=>card.IsSelected).ToList();
            var totalScore = 0;

            foreach (var card in selectedCards)
            {
                totalScore += card.AttackPts;
                addingScoreToUIAction?.Invoke(totalScore);
                AnimateShowScore(card, null);
                yield return new WaitForSeconds(k_ShowScoreTime);
            }
            
            m_scoreManager.AddScoresFromCard(totalScore);
            onFinish?.Invoke();
            
            DiscardSelectedCards();
            
            InputSystem.actions.Enable();
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

        private void SwapCard(CardBehavior card1, CardBehavior card2)
        {
            var prevCard1Pos = m_handPositions[card1.CardIndex].position;
            var prevCard2Pos = m_handPositions[card2.CardIndex].position;
            
            var card1Index = card1.CardIndex;
            var card2Index = card2.CardIndex;
            
            
            card1.TweenCardToPosition(prevCard2Pos, () =>
            {
                card1.transform.position = prevCard2Pos;
                card1.SetHandPosition(prevCard2Pos);
                card1.SetCardIndex(card2Index);
                card1.SetName();
            });
            card2.TweenCardToPosition(prevCard1Pos, () =>
            {
                card2.transform.position = prevCard1Pos;
                card2.SetHandPosition(prevCard1Pos);
                card2.SetCardIndex(card1Index);
                card2.SetName();
            });
        }
        
        /// <summary>
        /// Check if this card is overlapping any other card.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private CardBehavior IsCardOverlapping(CardBehavior card)
        {
            Collider2D[] results = new Collider2D[3]; // Adjust size as needed
            ContactFilter2D filter = new ContactFilter2D();
            int count = Physics2D.OverlapBox(card.CardCollider.bounds.center, card.CardCollider.size, 0,filter, results);
    
            for (int i = 0; i < count; i++)
            {
                // Skip if it's the same collider as the card we're checking
                if (results[i] == card.CardCollider) continue;
        
                if (results[i].gameObject.TryGetComponent(out CardBehavior otherCard))
                {
                    return otherCard;
                }
            }
    
            return null;

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
                .setOnComplete(()=>
                {
                    card.transform.position = m_handPositions[handIndex].position;
                    card.SetHandPosition(m_handPositions[handIndex].position);
                });
        }

        private void AnimateShowScore(CardBehavior cardBehavior, Action onFinish)
        {
            cardBehavior.ShowAtkPointHUD();
            cardBehavior.gameObject.LeanDelayedCall(
                k_ShowScoreTime,
                () =>
                {
                    cardBehavior.HideAtkPointHUD();
                    onFinish?.Invoke();
                });
        }
    }
}
