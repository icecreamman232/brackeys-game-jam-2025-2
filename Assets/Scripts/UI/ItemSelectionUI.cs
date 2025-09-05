using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SGGames.Scripts.UI
{
    public class ItemSelectionUI : Selectable
    {
        [SerializeField] private bool m_canShowDestroyButton;
        [SerializeField] private Image m_itemImage;
        [SerializeField] private GameObject m_selectionBG;
        [SerializeField] private ButtonController m_destroyButton;
        [SerializeField] private ItemDescriptionUI m_itemDescriptionUI;
        [SerializeField] private ItemData m_itemData;
    
        private bool m_isSelected;
        public bool IsSelected => m_isSelected;
    
        public Action<ItemData> OnClickAction;
        public Action<ItemSelectionUI> OnDestroyAction;
        
        public ItemData ItemData => m_itemData;

        protected override void Awake()
        {
            base.Awake();
            m_destroyButton.gameObject.SetActive(false);
            m_destroyButton.OnClickAction = OnPressDestroyButton;
        }

        public void SetItemData(ItemData itemData)
        {
            if (itemData != null)
            {
                m_itemImage.sprite = itemData.Icon;
            }
            
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

        private void OnSelect()
        {
            m_selectionBG.SetActive(true);
            if (m_canShowDestroyButton)
            {
                m_destroyButton.gameObject.SetActive(true);
            }
        }

        private void OnDeselect()
        {
            m_selectionBG.SetActive(false);
            if (m_canShowDestroyButton)
            {
                m_destroyButton.gameObject.SetActive(false);
            }
        }

        private void OnPressDestroyButton()
        {
            OnDestroyAction?.Invoke(this);
        }
    }
}

