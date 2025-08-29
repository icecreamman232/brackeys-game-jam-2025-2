using System.Collections.Generic;
using SGGames.Scripts.Card;

public enum CardComboRuleType
{
    None,
    OnePair,
    ElementOnePair,
    TwoPair,
    ElementTwoPair,
    ElementThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    ElementStraight,
}

public abstract class CardComboRule
{
    private CardComboRuleType m_ruleType;
    
    public CardComboRuleType RuleType => m_ruleType;
    
    public CardComboRule(CardComboRuleType ruleType)
    {
        m_ruleType = ruleType;
    }
    
    public abstract bool IsMatch(List<CardBehavior> selectedCards);
    
    protected bool HaveSameElement(List<CardBehavior> selectedCards)
    {
        var firstCard = selectedCards[0];
        for (int i = 1; i < selectedCards.Count; i++)
        {
            if (selectedCards[i].CardData.Info.Element != firstCard.CardData.Info.Element)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Check if selected cards is star card (straight in Poker)
    /// </summary>
    /// <param name="selectedCards"></param>
    /// <returns></returns>
    protected bool IsStraight(List<CardBehavior> selectedCards)
    {
        if (selectedCards.Count != 5)
            return false;

        // Extract attack points and sort them
        var attackPoints = new int[5];
        for (int i = 0; i < selectedCards.Count; i++)
        {
            attackPoints[i] = selectedCards[i].CardData.Info.AttackPoint;
        }
        
        System.Array.Sort(attackPoints);

        // Check if sorted attack points form sequence 1,2,3,4,5
        for (int i = 0; i < attackPoints.Length; i++)
        {
            if (attackPoints[i] != i + 1)
            {
                return false;
            }
        }
        
        return true;

    }
}
