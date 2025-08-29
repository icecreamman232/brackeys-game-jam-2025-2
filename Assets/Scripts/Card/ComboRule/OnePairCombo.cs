using System.Collections.Generic;
using SGGames.Scripts.Card;

public class OnePairCombo : CardComboRule
{
    public OnePairCombo(CardComboRuleType ruleType) : base(ruleType)
    {
    }

    public override bool IsMatch(List<CardBehavior> selectedCards)
    {
        if (selectedCards.Count < 2) return false;

        // Count occurrences of each attack point
        var attackPointCounts = new Dictionary<int, int>();
    
        foreach (var card in selectedCards)
        {
            int attackPoint = card.CardData.Info.AttackPoint;
            if (attackPointCounts.ContainsKey(attackPoint))
            {
                attackPointCounts[attackPoint]++;
            }
            else
            {
                attackPointCounts[attackPoint] = 1;
            }
        }
    
        // Check if there's exactly one pair (count of 2) and no higher matches
        int pairCount = 0;
        foreach (var count in attackPointCounts.Values)
        {
            if (count == 2)
            {
                pairCount++;
            }
            else if (count > 2)
            {
                return false; // Three of a kind or higher, not a pair
            }
        }
    
        return pairCount == 1; // Exactly one pair

    }
}
