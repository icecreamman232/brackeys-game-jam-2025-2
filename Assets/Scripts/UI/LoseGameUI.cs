using System;
using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using SGGames.Scripts.UI;
using UnityEngine;

public class LoseGameUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private RectTransform m_titleTransform;
    [SerializeField] private ButtonController m_mainMenuButton;
    private Vector3 m_originalTitlePosition;   
    
    public Action OnLoadToMainMenuAction;

    private void Awake()
    {
        m_mainMenuButton.OnClickAction = OnLoadToMainMenuAction;
        m_originalTitlePosition = m_titleTransform.localPosition;
        HidePanel();
    }

    private void LoadToMainMenu()
    {
        
    }

    [ContextMenu("Show Panel")]
    public void ShowPanel()
    {
        InputManager.SetActive(false);
        m_canvasGroup.Activate();
        m_titleTransform.LeanMoveLocalY(0, 1)
            .setOnComplete(() =>
            {
                m_mainMenuButton.gameObject.SetActive(true);
            });
    }

    public void HidePanel()
    {
        m_canvasGroup.Deactivate();
        m_titleTransform.localPosition = m_originalTitlePosition;
        m_mainMenuButton.gameObject.SetActive(false);
    }
}