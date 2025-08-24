using UnityEngine;

namespace SGGames.Scripts.Card
{
    public class CardPile : MonoBehaviour
    {
        [SerializeField] private int m_numberCardsInHand;
        [SerializeField] private CardBehavior m_cardPrefab;
        [SerializeField] private Transform m_handTransform;
        [SerializeField] private Transform[] m_cardPositions;
        
        private const float k_MovingToPositionTime = 0.7f;
        private const float k_MovingToPositionDelay = 0.05f;
        
        private void Start()
        {
            DealCards();
        }

        private void DealCards()
        {
            for (int i = 0; i < m_numberCardsInHand; i++)
            {
                var newCard = Instantiate(m_cardPrefab, m_handTransform);
                newCard.transform.position = this.transform.position;
                var index = i;
                newCard.transform.LeanMove(m_cardPositions[i].position, k_MovingToPositionTime)
                    .setEase(LeanTweenType.easeOutCubic)
                    .setDelay(k_MovingToPositionDelay * i)
                    .setOnComplete(
                    ()=> { newCard.transform.position = m_cardPositions[index].position;});
            }
        }
    }
}
