using System.Collections.Generic;
using SGGames.Scripts.Card;

public class ElementOnePairCombo : CardComboRule
{
    public ElementOnePairCombo(CardComboRuleType ruleType,int bonusEnergy) : base(ruleType, bonusEnergy)
    {
    }

    public override bool IsMatch(List<CardBehavior> selectedCards)
    {
        if (selectedCards.Count < 2) return false;
    
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

        // Check if any attack point and element combination appears at least 2 times
        foreach (var count in cardCombinations.Values)
        {
            if (count >= 2)
            {
                return true;
            }
        }

        return false;

    }
}
