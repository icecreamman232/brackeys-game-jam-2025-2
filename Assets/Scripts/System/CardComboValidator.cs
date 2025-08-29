using System.Collections.Generic;
using SGGames.Scripts.Card;

public class CardComboValidator
{
    private CardComboRule[] m_cardComboRules;

    public CardComboValidator()
    {
        m_cardComboRules = new CardComboRule[]
        {
            new OnePairCombo(CardComboRuleType.OnePair),
            new ElementOnePairCombo(CardComboRuleType.ElementOnePair),
            new TwoPairCombo(CardComboRuleType.TwoPair),
            new ElementTwoPairCombo(CardComboRuleType.ElementTwoPair),
            new ThreeOfAKindCombo(CardComboRuleType.ElementThreeOfAKind),
            new StraightCardCombo(CardComboRuleType.Straight), //Straight
            new ElementStraightCardCombo(CardComboRuleType.ElementStraight), //Straight Flush
        };
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
