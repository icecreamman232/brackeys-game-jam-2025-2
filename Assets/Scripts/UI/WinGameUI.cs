using SGGames.Scripts.Card;
using SGGames.Scripts.Core;
using SGGames.Scripts.Data;
using SGGames.Scripts.Managers;
using SGGames.Scripts.UI;
using UnityEngine;

public class WinGameUI : MonoBehaviour
{
    [Header("Title")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private RectTransform m_titleTransform;
    [SerializeField] private ButtonController m_nextButton;
    [Header("Item Selection")]
    [SerializeField] private CanvasGroup m_itemSelectionCanvasGroup;
    [SerializeField] private ItemSelectionUI[] m_itemSelectionUIList;
    [Header("Card Selection")]
    [SerializeField] private CanvasGroup m_cardSelectionCanvasGroup;
    [SerializeField] private CardPile m_cardPile;
    [SerializeField] private CardSelectionUI[] m_cardSelectionUIList;
    
    private Vector3 m_originalTitlePosition;
    private ItemData m_selectedItem;
    private CardData m_selectedCard;
    
    private void Awake()
    {
        m_originalTitlePosition = m_titleTransform.localPosition;
        foreach (var itemSelection in m_itemSelectionUIList)
        {
            itemSelection.OnClickAction = OnClickItemSelection;
        }

        foreach (var cardSelection in m_cardSelectionUIList)
        {
            cardSelection.OnClickAction = OnClickCardSelection;
        }
        
        HidePanel();
    }


    [ContextMenu("Show Panel")]
    public void ShowPanel()
    {
        InputManager.SetActive(false);
        m_nextButton.OnClickAction = ShowCardSelection;
        m_canvasGroup.Activate();
        m_cardSelectionCanvasGroup.Deactivate();
        m_titleTransform.LeanMoveLocalY(350, 0.5f)
            .setEase(LeanTweenType.easeOutCirc)
            .setOnComplete(ShowItemSelection);
        m_nextButton.gameObject.SetActive(false);
    }
    
    public void HidePanel()
    {
        m_canvasGroup.Deactivate();
        HideItemSelection();
        HideCardSelection();
        m_titleTransform.localPosition = m_originalTitlePosition;
        m_nextButton.gameObject.SetActive(false);
    }

    private void ShowItemSelection()
    {
        var itemManager = ServiceLocator.GetService<ItemManager>();
        var itemSelections = itemManager.GetRandomItemsWithoutDuplicates(3);
        for (int i = 0; i < itemSelections.Count; i++)
        {
            var itemData  = itemSelections[i];
            m_itemSelectionUIList[i].SetItemData(itemData);
        }
        m_itemSelectionCanvasGroup.Activate();
        
        for (int i = 0; i < itemSelections.Count; i++)
        {
            ((RectTransform)m_itemSelectionUIList[i].transform).LeanMoveLocalY(0, 0.2f)
                .setEase(LeanTweenType.easeOutCirc)
                .setDelay(i * 0.15f);
        }
    }

    private void HideItemSelection()
    {
        foreach (var itemSelection in m_itemSelectionUIList)
        {
            itemSelection.SetSelect(false);
        }
        m_itemSelectionCanvasGroup.Deactivate();
    }

    private void ShowCardSelection()
    {
        m_nextButton.OnClickAction = LoadNextLevel;
        HideItemSelection();
        var cardSelections = m_cardPile.GetCardsData(3);
        for (int i = 0; i < cardSelections.Count; i++)
        {
            var cardData  = cardSelections[i];
            m_cardSelectionUIList[i].SetCard(cardData);
        }
        
        m_cardSelectionCanvasGroup.Activate();
        
        for (int i = 0; i < cardSelections.Count; i++)
        {
            ((RectTransform)m_cardSelectionUIList[i].transform).LeanMoveLocalY(0, 0.2f)
                .setEase(LeanTweenType.easeOutCirc)
                .setDelay(i * 0.15f);
        }
    }

    private void HideCardSelection()
    {
        foreach (var cardSelection in m_cardSelectionUIList)
        {
            cardSelection.SetSelect(false);
        }
        
        m_cardSelectionCanvasGroup.Deactivate();
    }

    private void OnClickItemSelection(ItemData item)
    {
        m_nextButton.gameObject.SetActive(true);
        foreach (var itemSelection in m_itemSelectionUIList)
        {
            if (itemSelection.ItemData.Name == item.Name)
            {
                itemSelection.SetSelect(true);
                m_selectedItem = item;
            }
            else
            {
                itemSelection.SetSelect(false);
            }
        }
    }
    
    private void OnClickCardSelection(CardData cardData)
    {
        foreach (var cardSelection in m_cardSelectionUIList)
        {
            if (cardSelection.CardData.Name == cardData.Name)
            {
                cardSelection.SetSelect(true);
                m_selectedCard = cardData;
            }
            else
            {
                cardSelection.SetSelect(false);
            }
        }
    }

    private void LoadNextLevel()
    {
        HidePanel();
        //Add selected item to item manager
        ServiceLocator.GetService<ItemManager>().AddItem(m_selectedItem);
        m_cardPile.AddNewCard(m_selectedCard);
        //Call level manager to load next level
        ServiceLocator.GetService<LevelManager>().LoadNextLevel();
    }
}
