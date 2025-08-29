using System.Collections.Generic;
using SGGames.Scripts.Card;

/// <summary>
/// Five cards must be in increased order and must have same element.
/// </summary>
public class ElementStraightCardCombo : CardComboRule
{
    public ElementStraightCardCombo(CardComboRuleType ruleType) : base(ruleType)
    {
    }

    public override bool IsMatch(List<CardBehavior> selectedCards)
    {
        if (selectedCards.Count < 5) return false;
        if (!HaveSameElement(selectedCards)|| !IsStraight(selectedCards)) return false;
        
        return true;
    }
}
