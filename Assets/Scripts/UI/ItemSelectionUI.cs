using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelectionUI : Selectable
{
    [SerializeField] private Image m_itemImage;
    [SerializeField] private ItemDescriptionUI m_itemDescriptionUI;
    [SerializeField] private ItemData m_itemData;
    
    private bool m_isSelected;
    
    public Action<ItemData> OnClickAction;
    public ItemData ItemData => m_itemData;

    public void SetItemData(ItemData itemData)
    {
        m_itemImage.sprite = itemData.Icon;
        m_itemData = itemData;
        m_itemDescriptionUI.HideDescription();
    }

    public void SetSelect(bool isSelected)
    {
        m_isSelected = isSelected;
        if (isSelected)
        {
            OnSelect();
        }
        else
        {
            OnDeselect();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnClickAction?.Invoke(m_itemData);
        base.OnPointerDown(eventData);
    }

    private void OnSelect()
    {
        m_itemImage.rectTransform.LeanScale(Vector3.one * 1.3f, 0.2f)
            .setEase(LeanTweenType.easeOutExpo)
            .setOnComplete(() =>
            {
                m_itemDescriptionUI.ShowDescription(m_itemData.Name, m_itemData.Description, m_itemData.Rarity);
            });
    }

    private void OnDeselect()
    {
        m_itemImage.rectTransform.localScale = Vector3.one;
        m_itemDescriptionUI.HideDescription();
    }
}
