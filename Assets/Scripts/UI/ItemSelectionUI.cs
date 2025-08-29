using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelectionUI : Selectable
{
    [SerializeField] private Image m_itemImage;
    [SerializeField] private ItemDescriptionUI m_itemDescriptionUI;
    [SerializeField] private ItemData m_itemData;
    
    private bool m_isSelected;

    public void SetItemData(ItemData itemData)
    {
        m_itemImage.sprite = itemData.Icon;
        m_itemData = itemData;
        m_itemDescriptionUI.HideDescription();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        m_isSelected = true;
        base.OnPointerDown(eventData);
    }
    

    public override void OnPointerEnter(PointerEventData eventData)
    {
        m_itemDescriptionUI.ShowDescription(m_itemData.Name, m_itemData.Description, m_itemData.Rarity);
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        m_itemDescriptionUI.HideDescription();
        base.OnPointerExit(eventData);
    }
}
