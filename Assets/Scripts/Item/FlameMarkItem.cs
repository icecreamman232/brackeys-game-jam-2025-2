using SGGames.Scripts.Data;
using SGGames.Scripts.System;

public class FlameMarkItem : ItemBehavior
{
    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        var selectedCards = cardManager.SelectedCards;
        int numberOfFireElementCard = 0;
        foreach (var card in selectedCards)
        {
            if (card.CardData.Info.Element == CardElement.Fire)
            {
                numberOfFireElementCard++;
            }
        }

        if (numberOfFireElementCard > 0)
        {
            return (m_itemData.MultiplierType, m_itemData.MultiplierValue * numberOfFireElementCard);
        }
        return DefaultItemValue;
    }
}
