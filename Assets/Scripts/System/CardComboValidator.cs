using System.Collections.Generic;
using SGGames.Scripts.Card;

public class CardComboValidator
{
    private CardComboRule[] m_cardComboRules;

    public CardComboValidator()
    {
        m_cardComboRules = new CardComboRule[]
        {
            new ElementStraightCardCombo(CardComboRuleType.ElementStraight, 5), //Straight Flush
            new StraightCardCombo(CardComboRuleType.Straight, 4), //Straight
            new ElementThreeOfAKindCombo(CardComboRuleType.ElementThreeOfAKind, 3),
            new ElementTwoPairCombo(CardComboRuleType.ElementTwoPair, 2),
            new TwoPairCombo(CardComboRuleType.TwoPair, 1),
            new ElementOnePairCombo(CardComboRuleType.ElementOnePair, 1),
            new OnePairCombo(CardComboRuleType.OnePair, 0),
        };
    }

    public int GetComboBonusEnergy(CardComboRuleType ruleType)
    {
        foreach(var rule in m_cardComboRules)
        {
            if(rule.RuleType == ruleType)
            {
                return rule.BonusEnergy;
            }
        }
        
        return 0;
    }

    public CardComboRuleType IsMatch(List<CardBehavior> selectedCards)
    {
        foreach(var rule in m_cardComboRules)
        {
            if(rule.IsMatch(selectedCards))
            {
                return rule.RuleType;
            }
        }
        
        return CardComboRuleType.None;
    }
    
}
