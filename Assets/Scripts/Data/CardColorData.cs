using System;
using UnityEngine;

namespace SGGames.Scripts.Data
{
    [CreateAssetMenu(fileName = "Card Color Data", menuName = "SGGames/Card Color Data")]
    public class CardColorData : ScriptableObject
    {
        [SerializeField] private Color m_flameCardTextColor;
        [SerializeField] private Color m_waterCardTextColor;
        [SerializeField] private Color m_thunderCardTextColor;
    
        public Color FlameCardTextColor => m_flameCardTextColor;
        public Color WaterCardTextColor => m_waterCardTextColor;
        public Color ThunderCardTextColor => m_thunderCardTextColor;

        public Color GetTextColor(CardElement element)
        {
            switch (element)
            {
                case CardElement.Fire:
                    return m_flameCardTextColor;
                case CardElement.Water:
                    return m_waterCardTextColor;
                case CardElement.Thunder:
                    return m_thunderCardTextColor;
            }

            return Color.black;
        }
    }
}
