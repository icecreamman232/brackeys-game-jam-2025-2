using SGGames.Scripts.System;
using UnityEngine;

namespace SGGames.Scripts.Item
{
    public class DoubleMachineItem : ItemBehavior
    {
        public override (MultiplierType type, float value) Use(CardManager cardManager)
        {
            if (HasAtLeastTwoIdenticalCardsPlayed(cardManager))
            {
                return base.Use(cardManager);
            }
            
            return DefaultItemValue;
        }

        private bool HasAtLeastTwoIdenticalCardsPlayed(CardManager cardManager)
        {
            var selectedCards = cardManager.SelectedCards;
            
            for (int i = 0; i < selectedCards.Count; i++)
            {
                for (int j = i + 1; j < selectedCards.Count; j++)
                {
                    if (selectedCards[i].CardData.Name == selectedCards[j].CardData.Name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
