using SGGames.Scripts.Card;
using UnityEngine;

namespace SGGames.Scripts.Data
{
    public enum CardType
    {
        Attack,
        Skill,
    }
    
    [CreateAssetMenu(fileName = "Card Data", menuName = "SGGames/Card Data")]
    public class CardData : ScriptableObject
    {
        public CardType Type;
        public Sprite Icon;
        public CardBehavior CardPrefab;
        [TextArea(3, 10)]
        public string Description;
    }
}
