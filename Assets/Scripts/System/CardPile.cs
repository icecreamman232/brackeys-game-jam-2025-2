using System.Collections.Generic;
using SGGames.Scripts.Data;
using SGGames.Scripts.System;
using UnityEngine;

namespace SGGames.Scripts.Card
{
    public class CardPile : MonoBehaviour
    {
        [SerializeField] private CardManager m_cardManager;
        [SerializeField] private CardContainer m_cardContainer;
        
        private List<CardData> m_usedCards = new List<CardData>(); // Track used cards
        private List<CardBehavior> m_cardsInPile = new List<CardBehavior>();
        private const int k_DefaultCardCollectionSize = 12;
        
        public int CardCount => m_cardsInPile.Count;
        
        public CardBehavior DrawCard()
        {
            if (m_cardsInPile.Count == 0) return null;
            
            var card = m_cardsInPile[0];
            m_cardsInPile.RemoveAt(0);
            
            return card;
        }

        public List<CardBehavior> DrawCards(int number)
        {
            var drawnCard = new List<CardBehavior>();
            for (int i = 0; i < number; i++)
            {
                var card = DrawCard();
                drawnCard.Add(card);
            }
            
            return drawnCard;
        }

        public void AddCardsFromDiscard(List<CardBehavior> cards)
        {
            foreach (var card in cards)
            {
                AddCardToPile(card);
            }
        }
        
        public void InitializePile()
        {
            for (int i = 0; i < k_DefaultCardCollectionSize; i++)
            {
                var data = GetRandomCard();
                var newCard = CreateCard(data);
                AddCardToPile(newCard);
            }
        }
        
        public void AddNewCard(CardData data)
        {
            var newCard = CreateCard(data);
            AddCardToPile(newCard);
            Debug.Log($"New card {data.Name} added");
        }

        public List<CardData> GetCardsData(int number)
        {
            var cardsData = new List<CardData>();
            for (int i = 0; i < number; i++)
            {
                var card = GetRandomCard();
                cardsData.Add(card);
            }
            return cardsData;   
        }

        public void ResetPile()
        {
            foreach(var card in m_cardsInPile)
            {
                Destroy(card.gameObject);
            }
            m_cardsInPile.Clear();
        }

        private void AddCardToPile(CardBehavior card)
        {
            if (card == null)
            {
                Debug.LogError("Card is null");
                return;
            }
            
            m_cardsInPile.Add(card);
            card.ChangeCardState(CardState.InPile);
            card.transform.position = this.transform.position;
            card.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Get card prefab to create new card.
        /// </summary>
        /// <returns></returns>
        private CardData GetRandomCard()
        {
            // If all cards have been used, reset the used cards list
            if (m_usedCards.Count >= m_cardContainer.AttackCardList.Length)
            {
                m_usedCards.Clear();
            }

            // Create a list of available cards (not yet used)
            var availableCards = new List<CardData>();
            foreach (var card in m_cardContainer.AttackCardList)
            {
                if (!m_usedCards.Contains(card))
                {
                    availableCards.Add(card);
                }
            }

            // If no cards available (shouldn't happen due to reset above), fallback to original behavior
            if (availableCards.Count == 0)
            {
                return m_cardContainer.AttackCardList[Random.Range(0, m_cardContainer.AttackCardList.Length)];
            }

            // Select a random card from available cards
            var selectedCard = availableCards[Random.Range(0, availableCards.Count)];
            m_usedCards.Add(selectedCard);

            return selectedCard;
        }

        private CardBehavior CreateCard(CardData data)
        {
            var newCard = Instantiate(data.CardPrefab);
            newCard.transform.position = this.transform.position;
            newCard.SetData(data);
            newCard.CardIndex = -1;
            newCard.gameObject.SetActive(false);
            return newCard;
        }
    }
}
