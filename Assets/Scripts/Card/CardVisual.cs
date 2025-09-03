using SGGames.Scripts.Data;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private CardVisualContainer m_cardVisualContainer;
    [SerializeField] private CardColorData m_cardColorData;

    [Header("Card Visual")] 
    [SerializeField] private SpriteRenderer m_cardBG;
    [SerializeField] private SpriteRenderer m_atkPointBG;
    [SerializeField] private TextMeshProUGUI m_atkPointText;
    [SerializeField] private TextMeshProUGUI m_cardName;
    [SerializeField] private TextMeshProUGUI m_cardDesc;
    [SerializeField] private SpriteRenderer m_cardIcon;
    [SerializeField] private GameObject[] m_energyPoints;
    [Header("Visual Layer")]
    [SerializeField] private Renderer[] m_visualLayers;
    [SerializeField] private Canvas m_textCanvas;
    

    public void ChangeCardVisual(CardData data)
    {
        m_cardBG.sprite = m_cardVisualContainer.CardVisualList[(int)data.Info.Element].CardBG;
        m_atkPointBG.sprite = m_cardVisualContainer.CardVisualList[(int)data.Info.Element].AtkPointBG;
        
        m_atkPointText.text = data.Info.AttackPoint.ToString();
        m_cardName.text = data.Name;
        m_cardName.color = m_cardColorData.GetTextColor(data.Info.Element);
        m_cardDesc.text = data.Description;
        m_cardDesc.color = m_cardColorData.GetTextColor(data.Info.Element);
        m_cardIcon.sprite = data.Icon;
        for (int i = 0; i < data.Info.EnergyCost; i++)
        {
            m_energyPoints[i].SetActive(true);
        }
    }

    public void BringCardToFront()
    {
        foreach (var visual in m_visualLayers)
        {
            visual.sortingLayerName = "Dragging Card";
        }
        m_textCanvas.sortingLayerID = SortingLayer.NameToID("Dragging UI");
    }

    public void ResetToDefaultLayer()
    {
        foreach (var visual in m_visualLayers)
        {
            visual.sortingLayerName = "Default";
        }
        m_textCanvas.sortingLayerID = SortingLayer.NameToID("UI");
    }
}
