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

public enum ItemID
{
    AncientScript, DoubleMachine, EvenDay, OddDay, FlameMark,
    OceanMark, ThunderMark, HealthyGut, MegaSpeaker,
    MagicWand, BanhMi,
}

[CreateAssetMenu(fileName = "Item Data", menuName = "SGGames/Item Data")]
public class ItemData : ScriptableObject
{
    public ItemID ItemID;
    public string Name;
    [TextArea(3, 10)]
    public string Description;
    public ItemRarity Rarity;
    public MultiplierType MultiplierType;
    public float MultiplierValue;
    public Sprite Icon;
    public ItemBehavior ItemPrefab;
}
