using System;
using System.Collections.Generic;
using SGGames.Scripts.Card;
using SGGames.Scripts.Data;
using SGGames.Scripts.System;
using Random = UnityEngine.Random;

public enum BanhMiConditionType
{
    CountWaterCard,
    CountFireCard,
    CountThunderCard,
    COUNT
}

public class BanhMiItem : ItemBehavior
{
    private BanhMiConditionType m_currentConditionType;
    private int m_numberCardRequired;
    
    public (BanhMiConditionType conditionType, int numberRequired) CurrentCondition => (m_currentConditionType, m_numberCardRequired);

    private void Start()
    {
        RandomNewConditionType();
    }

    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        var selectedCards = cardManager.SelectedCards;
        bool isConditionMet = false;
        
        
        switch (m_currentConditionType)
        {
            case BanhMiConditionType.CountWaterCard:
                isConditionMet = HaveEnoughNumberOfCard(selectedCards, m_numberCardRequired, CardElement.Water);
                break;
            case BanhMiConditionType.CountFireCard:
                isConditionMet = HaveEnoughNumberOfCard(selectedCards, m_numberCardRequired, CardElement.Fire);
                break;
            case BanhMiConditionType.CountThunderCard:
                isConditionMet = HaveEnoughNumberOfCard(selectedCards, m_numberCardRequired, CardElement.Thunder);
                break;
        }

        if (isConditionMet)
        {
            return (m_itemData.MultiplierType, m_itemData.MultiplierValue);
        }

        return DefaultItemValue;
    }

    public void RandomNewConditionType()
    {
        m_currentConditionType = GetRandomCondition();
        m_numberCardRequired = Random.Range(1, 4);
    }

    private bool HaveEnoughNumberOfCard(List<CardBehavior> selectedCards, int requiredNumber, CardElement elementToCheck)
    {
        var cardCount = 0;
        foreach (var card in selectedCards)
        {
            if (card.CardData.Info.Element == elementToCheck)
            {
                cardCount++;
            }
        }
        return cardCount >= requiredNumber;
    }
    
    
    private BanhMiConditionType GetRandomCondition()
    {
        return (BanhMiConditionType)Random.Range(0, (int)BanhMiConditionType.COUNT);
    }
}
