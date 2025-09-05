using TMPro;
using UnityEngine;

namespace SGGames.Scripts.UI
{
    public class ItemDescriptionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_itemName;
        [SerializeField] private TextMeshProUGUI m_descriptionText;

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
}

