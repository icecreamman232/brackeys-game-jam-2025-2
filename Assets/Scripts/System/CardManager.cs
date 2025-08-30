using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SGGames.Scripts.Card;
using SGGames.Scripts.Core;
using UnityEngine;

namespace SGGames.Scripts.System
{
    public class CardManager : MonoBehaviour, IBootStrap, IGameService
    {
        [Header("Discard")]
        [SerializeField] private int m_maxDiscardTime;
        [SerializeField] private int m_currentDiscardNumber;
        [SerializeField] private DiscardNumberEvent m_discardNumberEvent;
        [SerializeField] private int m_currentTurnNumber;
        [SerializeField] private ScoreManager m_scoreManager;
        [SerializeField] private ItemManager m_itemManager;
        [SerializeField] private int m_maxHandSize = 5;
        [SerializeField] private Transform[] m_handPositions;
        [SerializeField] private List<CardBehavior> m_cardsInHand;
        [SerializeField] private CardPile m_cardPile;
        [SerializeField] private DiscardPile m_discardPile;
        [SerializeField] private GameEvent m_gameEvent;

        
        private Action<int, int> m_addingScoreToScoreDisplayAction;
        private CardComboRuleType m_currentComboType = CardComboRuleType.None;
        private CardComboValidator m_cardComboValidator;
        private EnergyManager m_energyManager;
        private const float k_MovingToPositionTime = 0.7f;
        private const float k_MovingToPositionDelay = 0.05f;
        private const float k_DiscardMoveTime = 0.3f;
        public const float k_ShowScoreTime = 0.3f;

        public Action UpdateScoreToFinalScoreUIAction;
        public bool CanDiscardManually => m_currentDiscardNumber > 0;
        
        public int CurrentTurnNumber => m_currentTurnNumber;
        public int NumberComboHasBeenPlayed => m_cardComboValidator.ComboHasBeenPlayed;
        
        public List<CardBehavior> CardsInHand => m_cardsInHand;
        public List<CardBehavior> SelectedCards => m_cardsInHand.Where(card=>card.IsSelected).ToList();

        public void Install()
        {
            ServiceLocator.RegisterService<CardManager>(this);
            m_energyManager = ServiceLocator.GetService<EnergyManager>();
            m_gameEvent.AddListener(OnGameEventChanged);
            m_currentTurnNumber = 0;
            m_cardComboValidator = new CardComboValidator();
            m_currentDiscardNumber = m_maxDiscardTime;
            m_discardNumberEvent.Raise(m_currentDiscardNumber);
        }

        public void Uninstall()
        {
            ServiceLocator.UnregisterService<CardManager>();
            m_gameEvent.RemoveListener(OnGameEventChanged);
        }

        public void DealFirstHands()
        {
            m_cardPile.InitializePile();
            
            var cardsToDeal = m_cardPile.DrawCards(m_maxHandSize);
            for (int i = 0; i < cardsToDeal.Count; i++)
            {
                var card = cardsToDeal[i];
                AddCardToHand(card, i);
                AnimateCardToHand(card, i, k_MovingToPositionDelay * i);
            }
            if(m_itemManager.HasItem(ItemID.RedPaper))
            {
                AddDiscardNumber(1);
            }
        }

        public void Reset()
        {
            StopAllCoroutines();
            m_cardComboValidator.ResetComboCounter();
            m_currentTurnNumber = 0;
            m_cardPile.ResetPile();
            m_discardPile.ResetPile();
            foreach (var card in m_cardsInHand)
            {
                card.ResetSelection();
                Destroy(card.gameObject);
            }
            m_cardsInHand.Clear();
        }

        public void AddDiscardNumber(int number)
        {
            m_currentDiscardNumber += number;
            m_maxDiscardTime += number;
            m_discardNumberEvent.Raise(m_currentDiscardNumber);
        }
        
        private void AddCardToHand(CardBehavior card, int handIndex)
        {
            card.ChangeCardState(CardState.InHand);
            card.SetCardIndex(handIndex);
            card.SetName();
            card.IsOverlappedOnCard = IsCardOverlapping;
            card.SwapCardsAction = SwapCard;
            card.CanBeSelected = m_energyManager.CanSelectedThisCard;
            card.SelectAction = OnCardSelected;
            card.DeselectAction = OnCardDeselected;
            
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

        public void CountScoreForCardAtIndex(int index)
        {
            if (SelectedCards.Count <= 0) return;
            
            StartCoroutine(TriggerCardAtIndex(index));
        }

        public void CountScoreFromSelectedCards(Action<int, int> addingScoreToUIAction, Action onFinish)
        {
            if (m_addingScoreToScoreDisplayAction == null)
            {
                m_addingScoreToScoreDisplayAction = addingScoreToUIAction;
            }
            m_currentComboType = CardComboRuleType.None;
            StartCoroutine(OnCountingScore(addingScoreToUIAction, onFinish));
        }
        
        private IEnumerator OnCountingScore(Action<int, int> addingScoreToUIAction, Action onFinish)
        {
            var selectedCards = m_cardsInHand.Where(card=>card.IsSelected).ToList();
            var totalScore = 0;

            foreach (var card in selectedCards)
            {
                totalScore += card.AttackPts;
                var startScoreForAnimation = totalScore - 10;
                //Prevent negative score
                if (startScoreForAnimation < 0)
                {
                    startScoreForAnimation = totalScore;
                }
                addingScoreToUIAction?.Invoke(startScoreForAnimation, totalScore);
                AnimateShowScoreOnCard(card, null);
                yield return new WaitForSeconds(k_ShowScoreTime + 0.2f + 0.2f);
            }
            
            m_scoreManager.AddScoresFromCard(totalScore);
            onFinish?.Invoke();
        }

        private IEnumerator TriggerCardAtIndex(int index)
        {
            var card = SelectedCards[index];
            var score = card.AttackPts;
            m_scoreManager.AddScoresFromCard(score);
            var currentScore = m_scoreManager.Score;
            var startScoreForAnimation = currentScore - 10;
            if (startScoreForAnimation > 0)
            {
                startScoreForAnimation = currentScore;
            }
            m_addingScoreToScoreDisplayAction?.Invoke(startScoreForAnimation, currentScore);
            AnimateShowScoreOnCard(card, null);
            yield return new WaitForSeconds(k_ShowScoreTime + 0.2f + 0.2f);
        }

        public void DiscardSelectedCards(bool isManualDiscard)
        {
            m_currentComboType = CardComboRuleType.None;
            m_energyManager.Reset();
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

            if (isManualDiscard)
            {
                m_currentDiscardNumber--;
                m_discardNumberEvent.Raise(m_currentDiscardNumber);
                if (m_currentDiscardNumber <= 0)
                {
                    m_currentDiscardNumber = 0;
                }
            }
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
            
            //Random new Banh Mi condition after every turn
            m_itemManager.RandomNewBanhMiCondition();
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

        public void FinishTurn()
        {
            m_energyManager.Reset();
            m_currentTurnNumber++;
            DiscardSelectedCards(false);
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

        private void OnCardSelected()
        {
            UpdateComboBonus();
        }

        private void OnCardDeselected()
        {
            UpdateComboBonus();
        }
        
        private void UpdateComboBonus()
        {
            CardComboRuleType newComboType = m_cardComboValidator.IsMatch(SelectedCards);
    
            // Remove previous combo bonus if we had one
            if (m_currentComboType != CardComboRuleType.None)
            {
                var previousBonusEnergy = m_cardComboValidator.GetComboBonusEnergy(m_currentComboType);
                m_energyManager.RemoveEnergy(previousBonusEnergy);
            }
    
            // Add new combo bonus if we have one
            if (newComboType != CardComboRuleType.None)
            {
                var bonusEnergy = m_cardComboValidator.GetComboBonusEnergy(newComboType);
                m_energyManager.AddEnergy(bonusEnergy);
            }
    
            m_currentComboType = newComboType;
        }
        
        private void OnGameEventChanged(GameEventType eventType)
        {
            if (eventType == GameEventType.Victory)
            {
                Reset();
            }
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

        private void AnimateShowScoreOnCard(CardBehavior cardBehavior, Action onFinish)
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
