using UnityEngine;

public class ItemBehavior : MonoBehaviour, IItem
{
    [SerializeField] private ItemData m_itemData;
    
    public virtual void Use()
    {
        
    }
}
