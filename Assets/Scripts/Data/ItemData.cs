using UnityEngine;

public enum MultiplierType
{
    Add,
    Multiply,
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
}

[CreateAssetMenu(fileName = "Item Data", menuName = "SGGames/Item Data")]
public class ItemData : ScriptableObject
{
    public ItemRarity Rarity;
    public MultiplierType MultiplierType;
    public float MultiplierValue;
    public Sprite Icon;
}
