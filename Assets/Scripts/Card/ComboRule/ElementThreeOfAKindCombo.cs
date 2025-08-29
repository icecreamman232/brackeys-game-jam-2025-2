using System.Collections.Generic;
using SGGames.Scripts.Card;


/// <summary>
/// Rule 3 of A Kind: There is at least 3 cards that has same atk point and element
/// </summary>
public class ElementThreeOfAKindCombo : CardComboRule
{
    public ElementThreeOfAKindCombo(CardComboRuleType ruleType, int bonusEnergy) : base(ruleType, bonusEnergy)
    {
    }

    public override bool IsMatch(List<CardBehavior> selectedCards)
    {
        if (selectedCards.Count < 3) return false;
    
        // Count occurrences of each attack point and element combination
        var attackPointElementCounts = new Dictionary<(int attackPoint, int element), int>();

        foreach (var card in selectedCards)
        {
            int attackPoint = card.CardData.Info.AttackPoint;
            int element = (int)card.CardData.Info.Element;
            var key = (attackPoint, element);
        
            if (attackPointElementCounts.ContainsKey(key))
            {
                attackPointElementCounts[key]++;
            }
            else
            {
                attackPointElementCounts[key] = 1;
            }
        }

        // Check if any attack point and element combination appears at least 3 times
        foreach (var count in attackPointElementCounts.Values)
        {
            if (count >= 3)
            {
                return true;
            }
        }

        return false;

    }
    
}
