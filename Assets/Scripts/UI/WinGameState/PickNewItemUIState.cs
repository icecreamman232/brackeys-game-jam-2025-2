using System.Collections.Generic;
using SGGames.Scripts.Core;
using SGGames.Scripts.Item;
using UnityEngine;


namespace SGGames.Scripts.UI
{
    public class PickNewItemUIState : WinGameUIState
    {
        private CanvasGroup m_canvasGroup;
        private ButtonController m_nextButton;
        private ItemData m_selectedItem;
        private ItemSelectionUI[] m_newItemList;
        private ItemSelectionUI[] m_ownedItemList;
        private List<ItemData> m_itemToDestroyList = new List<ItemData>();
        
        public ItemData SelectedItem => m_selectedItem;

        public PickNewItemUIState(WinGameUIStateType stateType, CanvasGroup canvasGroup, ButtonController nextButton, 
            ItemSelectionUI[] ownedItemList ,ItemSelectionUI[] newItemList) 
            : base(stateType)
        {
            m_canvasGroup = canvasGroup;
            m_nextButton = nextButton;
            m_ownedItemList = ownedItemList;
            m_newItemList = newItemList;
            
            foreach (var itemSelection in m_ownedItemList)
            {
                itemSelection.OnClickAction = OnClickOwnedItemSelection;
                itemSelection.OnDestroyAction = OnPressDestroyButton;
            }
            
            
            foreach (var itemSelection in m_newItemList)
            {
                itemSelection.OnClickAction = OnClickNewItemSelection;
            }
            
        }
        
        public override void Initialize()
        {
            
        }

        public override void EnterState()
        {
            var itemManager = ServiceLocator.GetService<ItemManager>();
            var ownedItems = itemManager.OwnedItems;
            var newItem = itemManager.GetRandomItemsWithoutDuplicates(3);

            foreach (var ownedItem in m_ownedItemList)
            {
                ownedItem.gameObject.SetActive(false);
            }
            
            for (int i = 0; i < ownedItems.Count; i++)
            {
                var ownedItem = ownedItems[i];
                m_ownedItemList[i].gameObject.SetActive(true);
                m_ownedItemList[i].SetItemData(ownedItem.ItemData);
            }
            
            for (int i = 0; i < newItem.Count; i++)
            {
                var itemData  = newItem[i];
                m_newItemList[i].SetItemData(itemData);
            }
            
            m_canvasGroup.Activate();
            
            // for (int i = 0; i < newItem.Count; i++)
            // {
            //     ((RectTransform)m_newItemList[i].transform).LeanMoveLocalY(0, 0.2f)
            //         .setEase(LeanTweenType.easeOutCirc)
            //         .setDelay(i * 0.15f);
            // }
        }

        public override void ExitState()
        {
            foreach (var itemSelection in m_newItemList)
            {
                itemSelection.SetSelect(false);
            }
            m_canvasGroup.Deactivate();
        }

        public override void Hide()
        {
            foreach (var itemSelection in m_newItemList)
            {
                itemSelection.SetSelect(false);
            }
            
            foreach (var itemSelection in m_ownedItemList)
            {
                itemSelection.SetSelect(false);
            }
            
            m_canvasGroup.Deactivate();
        }

        private void OnClickOwnedItemSelection(ItemData item)
        {
            foreach (var itemSelection in m_ownedItemList)
            {
                if (itemSelection.ItemData == null)
                {
                    continue;
                }

                if (itemSelection.ItemData.Name == item.Name)
                {
                    itemSelection.SetSelect(!itemSelection.IsSelected);
                }
                else
                {
                    itemSelection.SetSelect(false);
                }
            }
        }

        private void OnClickNewItemSelection(ItemData item)
        {
            m_nextButton.gameObject.SetActive(true);
            foreach (var itemSelection in m_newItemList)
            {
                if (itemSelection.ItemData.Name == item.Name)
                {
                    if (itemSelection.IsSelected)
                    {
                        itemSelection.SetSelect(false);
                        m_selectedItem = null;
                    }
                    else
                    {
                        itemSelection.SetSelect(true);
                        m_selectedItem = item;
                    }
                }
                else
                {
                    itemSelection.SetSelect(false);
                }
            }
        }
        
        private void OnPressDestroyButton(ItemSelectionUI itemSelection)
        {
            m_itemToDestroyList.Add(itemSelection.ItemData);
            itemSelection.SetItemData(null);
            itemSelection.SetSelect(false);
            itemSelection.gameObject.SetActive(false);
        }
    }
}
