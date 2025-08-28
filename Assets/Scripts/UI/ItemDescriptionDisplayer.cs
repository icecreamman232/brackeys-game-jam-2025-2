using System;
using TMPro;
using UnityEngine;

public class ItemDescriptionDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_itemName;
    [SerializeField] private TextMeshPro m_descriptionText;

    public void ShowDescription(string itemName, string description, ItemRarity rarity)
    {
        this.gameObject.SetActive(true);
        m_itemName.text = itemName;
        m_descriptionText.text = description;
        switch (rarity)
        {
            case ItemRarity.Common:
                m_itemName.color = Color.yellow;
                break;
            case ItemRarity.Uncommon:
                m_itemName.color = Color.cyan;
                break;
            case ItemRarity.Rare:
                m_itemName.color = Color.magenta;
                break;
        }
    }
    
    public void HideDescription()
    {
        this.gameObject.SetActive(false);
    }
}
