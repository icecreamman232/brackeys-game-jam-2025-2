using System;
using SGGames.Scripts.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Visual Container", menuName = "SGGames/Card Visual Container")]
public class CardVisualContainer : ScriptableObject
{
    public CardVisualData[] CardVisualList;
}

[Serializable]
public class CardVisualData
{
    public CardElement CardElement;
    public Sprite CardBG;
    public Sprite AtkPointBG;
}
