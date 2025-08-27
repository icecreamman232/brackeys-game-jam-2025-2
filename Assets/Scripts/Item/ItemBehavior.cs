using System;
using SGGames.Scripts.System;
using UnityEngine;

public class ItemBehavior : MonoBehaviour, IItem
{
    [SerializeField] protected int m_itemIndex;
    [SerializeField] protected ItemData m_itemData;

    public int ItemIndex
    {
        get => m_itemIndex;
        set => m_itemIndex = value;
    }
    
    public virtual (MultiplierType type, float value) Use(CardManager cardManager)
    {
        return (m_itemData.MultiplierType, m_itemData.MultiplierValue);
    }

    private void OnMouseEnter()
    {
        transform.LeanScale(Vector3.one * 1.2f, 0.1f);
    }

    private void OnMouseExit()
    {
        transform.localScale = Vector3.one;
    }
}
