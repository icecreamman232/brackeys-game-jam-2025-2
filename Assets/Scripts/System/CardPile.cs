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
        [SerializeField] private int m_numberCardsInHand;
        [SerializeField] private Transform m_handTransform;
        [SerializeField] private Transform[] m_cardPositions;

        private const int k_DefaultCardCollectionSize = 12;
        private const float k_MovingToPositionTime = 0.7f;
        private const float k_MovingToPositionDelay = 0.05f;
        
        private void Start()
        {
            InitializeCardPile();
            DealAllCards();
        }
        
        public void DealCardIntoIndex(List<int> positionIndex)
        {
            //There is not enough card in the pile
            if (positionIndex.Count > m_cardManager.CardsInPile.Count)
            {
                m_cardManager.BringCardFromDiscardBack();
            }
            
            var listCardToHand = new List<CardBehavior>();
            for (int i = 0; i < positionIndex.Count; i++)
            {
                var newCard = m_cardManager.CardsInPile[i];
                newCard.transform.position = this.transform.position;
                newCard.SetCardIndex(positionIndex[i]);
                newCard.name = $"{newCard.name} - Index {i}";
                var index = positionIndex[i];
                newCard.transform.LeanMove(m_cardPositions[positionIndex[i]].position, k_MovingToPositionTime)
                    .setEase(LeanTweenType.easeOutCubic)
                    .setDelay(k_MovingToPositionDelay * positionIndex[i])
                    .setOnComplete(
                        ()=> { newCard.transform.position = m_cardPositions[index].position;});
                listCardToHand.Add(newCard);
                
                
                newCard.gameObject.SetActive(true);
            }

            for (int i = 0; i < listCardToHand.Count; i++)
            {
                m_cardManager.AddCardToHand(listCardToHand[i]);
            }
        }

        private void DealAllCards()
        {
            for (int i = 0; i < m_numberCardsInHand; i++)
            {
                var newCard = m_cardManager.CardsInPile[i];
                newCard.transform.position = this.transform.position;
                newCard.SetCardIndex(i);
                newCard.name = $"{newCard.name} - Index {i}";
                var index = i;
                newCard.transform.LeanMove(m_cardPositions[i].position, k_MovingToPositionTime)
                    .setEase(LeanTweenType.easeOutCubic)
                    .setDelay(k_MovingToPositionDelay * i)
                    .setOnComplete(
                    ()=> { newCard.transform.position = m_cardPositions[index].position;});
                m_cardManager.AddCardToHand(newCard);
                
                newCard.gameObject.SetActive(true);
            }
        }

        private void InitializeCardPile()
        {
            for (int i = 0; i < k_DefaultCardCollectionSize; i++)
            {
                var data = GetCard();
                var newCard = Instantiate(data.CardPrefab, m_handTransform);
                newCard.transform.position = this.transform.position;
                newCard.SetIcon(data.Icon);
                newCard.SetCardIndex(-1);
                newCard.gameObject.SetActive(false);
                m_cardManager.AddNewCardToPile(newCard);
            }
        }
        
        /// <summary>
        /// Get card prefab to create new card.
        /// </summary>
        /// <returns></returns>
        private CardData GetCard()
        {
            return m_cardContainer.AttackCardList[Random.Range(0, m_cardContainer.AttackCardList.Length)];
        }
    }
}
