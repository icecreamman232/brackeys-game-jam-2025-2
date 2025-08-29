using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using SGGames.Scripts.UI;
using UnityEngine;

public class WinGameUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private RectTransform m_titleTransform;
    [SerializeField] private CanvasGroup m_itemSelectionCanvasGroup;
    [SerializeField] private ButtonController m_nextButton;
    [SerializeField] private ItemSelectionUI[] m_itemSelectionUIList;
    private Vector3 m_originalTitlePosition;
    private ItemData m_selectedItem;
    
    private void Awake()
    {
        m_originalTitlePosition = m_titleTransform.localPosition;
        foreach (var itemSelection in m_itemSelectionUIList)
        {
            itemSelection.OnClickAction = OnClickItemSelection;
        }
        m_nextButton.OnClickAction = LoadNextLevel;
        HidePanel();
    }

    
    [ContextMenu("Show Panel")]
    public void ShowPanel()
    {
        InputManager.SetActive(false);
        m_canvasGroup.Activate();
        m_titleTransform.LeanMoveLocalY(350, 0.5f)
            .setEase(LeanTweenType.easeOutCirc)
            .setOnComplete(ShowItemSelection);
        m_nextButton.gameObject.SetActive(false);
    }
    
    public void HidePanel()
    {
        m_canvasGroup.Deactivate();
        m_itemSelectionCanvasGroup.Deactivate();
        m_titleTransform.localPosition = m_originalTitlePosition;
        m_nextButton.gameObject.SetActive(false);
    }

    private void ShowItemSelection()
    {
        var itemManager = ServiceLocator.GetService<ItemManager>();
        var itemSelections = itemManager.GetRandomItems(3);
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

    private void LoadNextLevel()
    {
        HidePanel();
        //Add selected item to item manager
        ServiceLocator.GetService<ItemManager>().AddItem(m_selectedItem);
        //Call level manager to load next level
        ServiceLocator.GetService<LevelManager>().LoadNextLevel();
    }
}
