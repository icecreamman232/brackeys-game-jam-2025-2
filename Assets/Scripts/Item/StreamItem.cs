using SGGames.Scripts.Data;
using SGGames.Scripts.System;


public class StreamItem : ItemBehavior
{
    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        var selectedCards = cardManager.SelectedCards;
        int numberOfWaterElementCard = 0;
        foreach (var card in selectedCards)
        {
            if (card.CardData.Info.Element == CardElement.Water)
            {
                numberOfWaterElementCard++;
            }
        }

        if (numberOfWaterElementCard > 0)
        {
            return (m_itemData.MultiplierType, m_itemData.MultiplierValue * numberOfWaterElementCard);
        }
        return DefaultItemValue;
    }
}
