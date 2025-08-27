using SGGames.Scripts.System;
using UnityEngine;

public class EvenDayItem : ItemBehavior
{
    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        bool isNumberSelectedEven = cardManager.SelectedCards.Count % 2 == 0;
        if (isNumberSelectedEven)
        {
            return base.Use(cardManager); 
        }
        return DefaultItemValue;
    }
}
