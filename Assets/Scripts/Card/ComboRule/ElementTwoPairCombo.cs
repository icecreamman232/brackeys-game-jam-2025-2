using System.Collections.Generic;
using SGGames.Scripts.Card;

public class ElementTwoPairCombo : CardComboRule
{
    public ElementTwoPairCombo(CardComboRuleType ruleType) : base(ruleType)
    {
    }

    public override bool IsMatch(List<CardBehavior> selectedCards)
    {
        if (selectedCards.Count < 4) return false;
    
        // Count occurrences of each attack point and element combination
        var cardCombinations = new Dictionary<(int attackPoint, int element), int>();

        foreach (var card in selectedCards)
        {
            int attackPoint = card.CardData.Info.AttackPoint;
            int element = (int)card.CardData.Info.Element;
            var key = (attackPoint, element);
        
            if (cardCombinations.ContainsKey(key))
            {
                cardCombinations[key]++;
            }
            else
            {
                cardCombinations[key] = 1;
            }
        }

        // Count how many pairs we have (groups with at least 2 cards of same attack point and element)
        int pairCount = 0;
        foreach (var count in cardCombinations.Values)
        {
            if (count >= 2)
            {
                pairCount++;
            }
        }

        // Check if we have at least 2 pairs
        return pairCount >= 2;
    }
}
