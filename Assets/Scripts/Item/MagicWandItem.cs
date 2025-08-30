using SGGames.Scripts.System;

public class MagicWandItem : ItemBehavior
{
  public override (MultiplierType type, float value) Use(CardManager cardManager)
  {
    var selectedCards = cardManager.SelectedCards;

    if (selectedCards.Count > 0)
    {
      var mul = selectedCards[^1].AttackPts;
      return (MultiplierType.Add, mul);
    }
    return DefaultItemValue;
  }
}
