using SGGames.Scripts.System;

public class OddDayItem : ItemBehavior
{
    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        bool isNumberSelectedOdd = cardManager.SelectedCards.Count % 2 == 1;
        if (isNumberSelectedOdd)
        {
            return base.Use(cardManager); 
        }
        return DefaultItemValue;
    }
}
