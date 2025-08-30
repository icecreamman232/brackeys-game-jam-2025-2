using SGGames.Scripts.System;

public class RedPaperItem : ItemBehavior
{
    public override (MultiplierType type, float value) Use(CardManager cardManager)
    {
        //Keep this empty for intention
        return (MultiplierType.Add, 0);
    }
}
