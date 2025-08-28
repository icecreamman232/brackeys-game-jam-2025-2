using System;
using SGGames.Scripts.Card;
using UnityEngine;

namespace SGGames.Scripts.Data
{
    public enum CardType
    {
        Attack,
        Skill,
    }

    public enum CardElement
    {
        Fire,
        Water,
        Thunder,
    }
    
    [CreateAssetMenu(fileName = "Card Data", menuName = "SGGames/Card Data")]
    public class CardData : ScriptableObject
    {
        public string Name;
        [TextArea(3, 10)]
        public string Description;
        public CardInfo Info;
        public Sprite Icon;
        public CardBehavior CardPrefab;
        
    }

    [Serializable]
    public class CardInfo
    {
        public CardType Type;
        public int EnergyCost;
        public int AttackPoint;
        public CardElement Element;
    }
}
