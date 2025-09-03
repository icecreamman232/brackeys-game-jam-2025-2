using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SGGames.Scripts.Card;
using SGGames.Scripts.Core;
using SGGames.Scripts.Data;
using SGGames.Scripts.Event;
using SGGames.Scripts.Item;
using SGGames.Scripts.Managers;
using UnityEngine;

namespace SGGames.Scripts.System
{
    public class CardManager : MonoBehaviour, IBootStrap, IGameService
    {
        [Header("Events")]
        [SerializeField] private PlaySFXEvent m_playSFXEvent;
        [SerializeField] private DiscardNumberEvent m_discardNumberEvent;
        [SerializeField] private GameEvent m_gameEvent;
        [SerializeField] private CountScoreEvent m_countScoreEvent;
        [Header("Components")]
        [SerializeField] private CardPile m_cardPile;
        [SerializeField] private DiscardPile m_discardPile;
        [Header("Data")]
        [SerializeField] private int m_maxDiscardTime;
        [SerializeField] private int m_currentDiscardNumber;
        [SerializeField] private int m_currentTurnNumber;
        [SerializeField] private int m_maxHandSize = 5;
        [SerializeField] private Transform[] m_handPositions;
        [SerializeField] private List<CardBehavior> m_cardsInHand;

        private int m_numberFireCardPlayed;
        private int m_numberThunderCardPlayed;
        private int m_numberWaterCardPlayed;
        private CardElement m_majorityElement;
        
        private ScoreManager m_scoreManager;
        private ItemManager m_itemManager;
        private CardComboValidator m_cardComboValidator;
        private EnergyManager m_energyManager;
        private Action<int, int> m_addingScoreToScoreDisplayAction;
        private CardComboRuleType m_currentComboType = CardComboRuleType.None;

        public Action UpdateScoreToFinalScoreUIAction;
        public bool CanDiscardManually => m_currentDiscardNumber > 0;
        public int NumberComboHasBeenPlayed => m_cardComboValidator.ComboHasBeenPlayed;
        public List<CardBehavior> SelectedCards => m_cardsInHand.Where(card=>card.IsSelected).ToList();
        public CardElement MajorityElement => m_majorityElement;
        
        private const float k_MovingToPositionTime = 0.7f;
        private const float k_MovingToPositionDelay = 0.05f;
        private const float k_DiscardMoveTime = 0.3f;
        public const float k_ShowScoreTime = 0.3f;
        
        private void Update()
        {
            if (Input.GetMouseButtonUp(1) && InputManager.IsActivated)
            {
                foreach (var card in SelectedCards)
                {
                    card.ResetSelection(shouldReturnEnergy:true);
                }
            }
        }

        public void Install()
        {
            ServiceLocator.RegisterService<CardManager>(this);
            m_energyManager = ServiceLocator.GetService<EnergyManager>();
            m_itemManager = ServiceLocator.GetService<ItemManager>();
            m_scoreManager = ServiceLocator.GetService<ScoreManager>();
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
        
        public void CountScoreForCardAtIndex(int index)
        {
            if (SelectedCards.Count <= 0) return;
            CalculateNumberElementCardPlayed();
            StartCoroutine(TriggerCardAtIndex(index));
        }

        public void CountScoreFromSelectedCards(Action<int, int> addingScoreToUIAction, Action onFinish)
        {
            CalculateNumberElementCardPlayed();
            if (m_addingScoreToScoreDisplayAction == null)
            {
                m_addingScoreToScoreDisplayAction = addingScoreToUIAction;
            }
            m_currentComboType = CardComboRuleType.None;
            StartCoroutine(OnCountingScore(addingScoreToUIAction, onFinish));
        }
        
        public void DiscardSelectedCards(bool isManualDiscard)
        {
            if (SelectedCards.Count <= 0)
            {
                m_playSFXEvent.Raise(SFX.ButtonCancel);
                return;
            }
            m_energyManager.Reset();
            m_currentComboType = CardComboRuleType.None;
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
        
        public void FinishTurn()
        {
            m_gameEvent.Raise(GameEventType.CheckMutation);
            m_currentTurnNumber++;
            DiscardSelectedCards(false);
        }
        
        private void AddCardToHand(CardBehavior card, int handIndex)
        {
            card.ChangeCardState(CardState.InHand);
            card.CardIndex = handIndex;
            card.SetName();
            card.IsOverlappedOnCard = IsCardOverlapping;
            card.SwapCardsAction = SwapCard;
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
                m_countScoreEvent.Raise(new CountScoreEventData
                {
                    //No need to insert score since card itself has score data
                    Score = -1,
                    PositionIndex = card.CardIndex,
                    EntityType = EntityType.Card
                });
                m_playSFXEvent.Raise(SFX.ScoreCounting);
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
            m_countScoreEvent.Raise(new CountScoreEventData
            {
                //No need to insert score since card itself has score data
                Score = -1, 
                PositionIndex = index,
                EntityType = EntityType.Card
            });

            yield return new WaitForSeconds(k_ShowScoreTime + 0.2f + 0.2f);
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
            
            
            card1.Input.CanClick = false;
            card2.Input.CanClick = false;
            
            card1.Animation.TweenCardToPosition(prevCard2Pos, () =>
            {
                card1.transform.position = prevCard2Pos;
                card1.SetHandPosition(prevCard2Pos);
                card1.CardIndex = card2Index;
                card1.SetName();
                card1.Input.CanClick = true;
            });
            card2.Animation.TweenCardToPosition(prevCard1Pos, () =>
            {
                card2.transform.position = prevCard1Pos;
                card2.SetHandPosition(prevCard1Pos);
                card2.CardIndex = card1Index;
                card2.SetName();
                card2.Input.CanClick = true;
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
            card.BringCardToFront(true);
            card.Input.CanClick = false;
            card.transform.LeanMove(m_discardPile.transform.position, k_DiscardMoveTime)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(() => 
                {
                    card.BringCardToFront(false);
                    card.Input.CanClick = true;
                    m_discardPile.AddCardToDiscard(card);
                    m_discardPile.PositionCardAtDiscard(card);
                });

        }
        
        private void AnimateCardToHand(CardBehavior card, int handIndex, float delay)
        {
            card.BringCardToFront(true);
            card.Input.CanClick = false;
            card.gameObject.SetActive(true);
            card.transform.LeanMove(m_handPositions[handIndex].position, k_MovingToPositionTime)
                .setEase(LeanTweenType.easeOutCubic)
                .setDelay(delay)
                .setOnComplete(()=>
                {
                    card.Input.CanClick = true;
                    card.BringCardToFront(false);
                    card.transform.position = m_handPositions[handIndex].position;
                    card.SetHandPosition(m_handPositions[handIndex].position);
                    card.Animation.SetOriginalPosition();
                });
        }

        private void CalculateNumberElementCardPlayed()
        {
            var selectedCards = SelectedCards;
            
            foreach (var card in selectedCards)
            {
                switch (card.CardData.Info.Element)
                {
                    case CardElement.Fire:
                        m_numberFireCardPlayed++;
                        break;
                    case CardElement.Water:
                        m_numberWaterCardPlayed++;
                        break;
                    case CardElement.Thunder:
                        m_numberThunderCardPlayed++;
                        break;
                }
            }
            var majorityElement = Mathf.Max(m_numberFireCardPlayed, Mathf.Max(m_numberWaterCardPlayed, m_numberThunderCardPlayed));
            if (majorityElement == m_numberFireCardPlayed)
            {
                m_majorityElement = CardElement.Fire;
            }
            else if (majorityElement == m_numberWaterCardPlayed)
            {
                m_majorityElement = CardElement.Water;
            }
            else if (majorityElement == m_numberThunderCardPlayed)
            {
                m_majorityElement = CardElement.Thunder;
            }
        }
    }
}
