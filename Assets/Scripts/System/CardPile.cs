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
            return m_cardContainer.AttackCardList[Random.Range(0, m_cardContainer.AttackCardList.Length)];
        }

        private CardBehavior CreateCard(CardData data)
        {
            var newCard = Instantiate(data.CardPrefab);
            newCard.transform.position = this.transform.position;
            newCard.SetIcon(data.Icon);
            newCard.SetAtkPoint(data.Info.AttackPoint);
            newCard.SetCardIndex(-1);
            newCard.gameObject.SetActive(false);
            return newCard;
        }
    }
}
