
using System.Collections.Generic;
using SGGames.Scripts.Card;
using SGGames.Scripts.Core;
using SGGames.Scripts.Item;
using SGGames.Scripts.Managers;
using UnityEngine;

namespace SGGames.Scripts.UI
{
    public class WinGameUI : MonoBehaviour
    {
        [Header("Title")]
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private RectTransform m_titleTransform;
        [SerializeField] private ButtonController m_nextButton;
        [Header("Item Selection")]
        [SerializeField] private CanvasGroup m_itemSelectionCanvasGroup;
        [SerializeField] private ItemSelectionUI[] m_currentItemList;
        [SerializeField] private ItemSelectionUI[] m_newItemList;
        [Header("Card Selection")]
        [SerializeField] private CanvasGroup m_cardSelectionCanvasGroup;
        [SerializeField] private CardPile m_cardPile;
        [SerializeField] private CardSelectionUI[] m_cardSelectionUIList;
        
        private Vector3 m_originalTitlePosition;
        private WinGameUIState m_currentState;
        private Dictionary<WinGameUIStateType, WinGameUIState> m_stateList;
        
        private void Awake()
        {
            m_originalTitlePosition = m_titleTransform.localPosition;
            
            m_nextButton.OnClickAction = OnClickNextButton;
            
            m_stateList = new Dictionary<WinGameUIStateType, WinGameUIState>();
            
            m_stateList.Add(WinGameUIStateType.PickNewItem, 
                new PickNewItemUIState(WinGameUIStateType.PickNewItem, m_itemSelectionCanvasGroup,m_nextButton, m_currentItemList, m_newItemList));
            m_stateList.Add(WinGameUIStateType.PickNewCard,
                new PickNewCardUIState(WinGameUIStateType.PickNewCard, m_cardSelectionCanvasGroup, m_cardPile, m_cardSelectionUIList));

            
            
            HidePanel();
        }


        [ContextMenu("Show Panel")]
        public void ShowPanel()
        {
            InputManager.SetActive(false);
            m_canvasGroup.Activate();
            m_titleTransform.localPosition = new Vector3(m_originalTitlePosition.x, m_originalTitlePosition.y - 30f, m_originalTitlePosition.z);
            m_titleTransform.LeanMoveLocalY(m_originalTitlePosition.y, 0.5f)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(ShowFirstUIState);
            m_nextButton.gameObject.SetActive(false);
        }
        
        public void HidePanel()
        {
            m_canvasGroup.Deactivate();
            foreach (var state in m_stateList)
            {
                state.Value.Hide();
            }
            m_titleTransform.localPosition = m_originalTitlePosition;
            m_nextButton.gameObject.SetActive(false);
        }

        private void ShowFirstUIState()
        {
            var itemNumber = ServiceLocator.GetService<ItemManager>().OwnedItems.Count;
            if (itemNumber > 0)
            {
                m_stateList[WinGameUIStateType.PickNewItem].EnterState();
            }
            else
            {
                //Show remove item panel
                
            }
        }

        private void OnClickNextButton()
        {
            switch (m_currentState.StateType)
            {
                case WinGameUIStateType.PickNewItem:
                    break;
                case WinGameUIStateType.PickNewCard:
                    break;
            }
        }

        
        private void LoadNextLevel()
        {
            HidePanel();
            //Add selected item to item manager
            var selectedItem = ((PickNewItemUIState)m_stateList[WinGameUIStateType.PickNewItem]).SelectedItem;
            var selectedCard = ((PickNewCardUIState)m_stateList[WinGameUIStateType.PickNewCard]).SelectedCard;
            ServiceLocator.GetService<ItemManager>().AddItem(selectedItem);
            m_cardPile.AddNewCard(selectedCard);
            //Call level manager to load next level
            ServiceLocator.GetService<LevelManager>().LoadNextLevel();
        }
    }
}

