using System.Collections.Generic;
using SGGames.Scripts.Card;

/// <summary>
/// 5 cars must be in increased order. This rule is similar to Straight.
/// </summary>
public class StraightCardCombo: CardComboRule
{
    public StraightCardCombo(CardComboRuleType ruleType, int bonusEnergy) : base(ruleType, bonusEnergy)
    {
    }

    public override bool IsMatch(List<CardBehavior> selectedCards)
    {
        if(selectedCards.Count < 5) return false;
        return IsStraight(selectedCards);
    }
}
