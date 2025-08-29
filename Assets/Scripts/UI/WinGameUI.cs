using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using UnityEngine;

public class WinGameUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private RectTransform m_titleTransform;
    [SerializeField] private CanvasGroup m_itemSelectionCanvasGroup;
    [SerializeField] private ItemSelectionUI[] m_itemSelectionUIList;
    private Vector3 m_originalTitlePosition;
    
    private void Awake()
    {
        m_originalTitlePosition = m_titleTransform.localPosition;
        HidePanel();
    }

    
    [ContextMenu("Show Panel")]
    public void ShowPanel()
    {
        InputManager.SetActive(false);
        m_canvasGroup.Activate();
        m_titleTransform.LeanMoveLocalY(282, 0.5f)
            .setOnComplete(ShowItemSelection);
    }
    
    public void HidePanel()
    {
        m_canvasGroup.Deactivate();
        m_itemSelectionCanvasGroup.Deactivate();
        m_titleTransform.localPosition = m_originalTitlePosition;
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
    }
}
