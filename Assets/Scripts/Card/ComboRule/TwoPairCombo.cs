using System.Collections.Generic;
using SGGames.Scripts.Card;

public class TwoPairCombo : CardComboRule
{
    public TwoPairCombo(CardComboRuleType ruleType, int bonusEnergy) : base(ruleType, bonusEnergy)
    {
    }

    public override bool IsMatch(List<CardBehavior> selectedCards)
    {
        if (selectedCards.Count < 4) return false;
        
        // Group cards by their attack points
        var attackPointGroups = new Dictionary<int, int>();
    
        foreach (var card in selectedCards)
        {
            int attackPoint = card.CardData.Info.AttackPoint;
            if (attackPointGroups.ContainsKey(attackPoint))
            {
                attackPointGroups[attackPoint]++;
            }
            else
            {
                attackPointGroups[attackPoint] = 1;
            }
        }

        // Count how many pairs we have (groups with at least 2 cards)
        int pairCount = 0;
        foreach (var group in attackPointGroups.Values)
        {
            if (group >= 2) 
            {
                pairCount++;
            }
        }

        // Check if we have at least 2 pairs
        return pairCount >= 2;

    }
}
