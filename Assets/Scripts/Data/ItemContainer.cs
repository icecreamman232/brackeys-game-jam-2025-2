using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Container", menuName = "SGGames/Item Container")]
public class ItemContainer : ScriptableObject
{
    public List<ItemBehavior> CommonItems;
    public List<ItemBehavior> UncommonItems;
    public List<ItemBehavior> RareItems;
}
