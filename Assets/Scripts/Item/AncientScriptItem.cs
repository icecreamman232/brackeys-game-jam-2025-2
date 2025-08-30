using SGGames.Scripts.System;

public class AncientScriptItem : ItemBehavior
{
    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        var selectedCards = cardManager.SelectedCards;
        if (selectedCards.Count <= 0) return DefaultItemValue;

        var totalComboHasBeenPlayed = cardManager.NumberComboHasBeenPlayed;

        if (totalComboHasBeenPlayed <= 0) return DefaultItemValue;
        
        return (m_itemData.MultiplierType, m_itemData.MultiplierValue * totalComboHasBeenPlayed);
    }
}
