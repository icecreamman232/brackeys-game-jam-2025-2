using System;
using SGGames.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelectionUI : Selectable
{
    [SerializeField] private CardVisualContainer m_cardVisualContainer;
    [SerializeField] private Image m_cardBG;
    [SerializeField] private Image m_cardIcon;
    [SerializeField] private Image m_atkPointBG;
    [SerializeField] private TextMeshProUGUI m_cardName;
    [SerializeField] private TextMeshProUGUI m_cardDesc;
    [SerializeField] private TextMeshProUGUI m_atkPointText;
    [SerializeField] private GameObject[] m_energyIcons;
    
    private CardData m_cardData;
    
    public CardData CardData => m_cardData;
    public Action<CardData> OnClickAction;
    
    public void SetCard(CardData data)
    {
        m_cardBG.sprite = m_cardVisualContainer.CardVisualList[(int)data.Info.Element].CardBG;
        m_atkPointBG.sprite = m_cardVisualContainer.CardVisualList[(int)data.Info.Element].AtkPointBG;
        
        m_atkPointText.text = data.Info.AttackPoint.ToString();
        m_cardIcon.sprite = data.Icon;
        m_cardName.text = data.Name;
        m_cardDesc.text = data.Description;
        
        for (int i = 0; i < data.Info.EnergyCost; i++)
        {
            m_energyIcons[i].SetActive(true);
        }

        m_cardData = data;
    }

    public void SetSelect(bool isSelected)
    {
        if (isSelected)
        {
            OnSelect();
        }
        else
        {
            OnDeselect();
        }
    }

    private void OnSelect()
    {
        ((RectTransform)this.transform).LeanScale(Vector3.one * 1.2f, 0.2f)
            .setEase(LeanTweenType.easeOutExpo);
    }

    private void OnDeselect()
    {
        ((RectTransform)this.transform).localScale = Vector3.one;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnClickAction?.Invoke(m_cardData);
        base.OnPointerDown(eventData);
    }
}
