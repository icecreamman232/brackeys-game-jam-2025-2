using SGGames.Scripts.Card;
using UnityEngine;

namespace SGGames.Scripts.Data
{
    [CreateAssetMenu(fileName = "Card Container", menuName = "SGGames/Card Container")]
    public class CardContainer : ScriptableObject
    {
        public CardData[] AttackCardList;
    }
}
