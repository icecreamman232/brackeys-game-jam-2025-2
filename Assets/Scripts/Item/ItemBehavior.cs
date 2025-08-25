using UnityEngine;

public class ItemBehavior : MonoBehaviour, IItem
{
    [SerializeField] private ItemData m_itemData;
    
    public virtual (MultiplierType type, float value) Use()
    {
        return (m_itemData.MultiplierType, m_itemData.MultiplierValue);
    }
}
