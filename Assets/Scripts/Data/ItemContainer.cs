using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Container", menuName = "SGGames/Item Container")]
public class ItemContainer : ScriptableObject
{
    public List<ItemData> CommonItems;
    public List<ItemData> UncommonItems;
    public List<ItemData> RareItems;
}
