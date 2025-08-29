using SGGames.Scripts.Data;
using SGGames.Scripts.System;

public class ThunderMarkItem : ItemBehavior
{
    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        var selectedCards = cardManager.SelectedCards;
        int numberOfThunderElementCard = 0;
        foreach (var card in selectedCards)
        {
            if (card.CardData.Info.Element == CardElement.Thunder)
            {
                numberOfThunderElementCard++;
            }
        }

        if (numberOfThunderElementCard > 0)
        {
            return (m_itemData.MultiplierType, m_itemData.MultiplierValue * numberOfThunderElementCard);
        }
        return DefaultItemValue;
    }
}
