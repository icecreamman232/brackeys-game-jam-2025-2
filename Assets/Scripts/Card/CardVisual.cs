using SGGames.Scripts.Data;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private CardVisualContainer m_cardVisualContainer;

    [Header("Card Visual")] 
    [SerializeField] private SpriteRenderer m_cardBG;
    [SerializeField] private SpriteRenderer m_atkPointBG;
    [SerializeField] private TextMeshPro m_atkPointText;
    [SerializeField] private TextMeshPro m_cardName;
    [SerializeField] private TextMeshPro m_cardDesc;
    [SerializeField] private SpriteRenderer m_cardIcon;
    [SerializeField] private GameObject[] m_energyPoints;
    [Header("Visual Layer")]
    [SerializeField] private Renderer[] m_visualLayers;
    [SerializeField] private TextMeshPro[] m_visualTexts;
    
    public void ChangeCardVisual(CardData data)
    {
        m_cardBG.sprite = m_cardVisualContainer.CardVisualList[(int)data.Info.Element].CardBG;
        m_atkPointBG.sprite = m_cardVisualContainer.CardVisualList[(int)data.Info.Element].AtkPointBG;
        
        m_atkPointText.text = data.Info.AttackPoint.ToString();
        m_cardName.text = data.Name;
        m_cardDesc.text = data.Description;
        m_cardIcon.sprite = data.Icon;
        for (int i = 0; i < data.Info.EnergyCost; i++)
        {
            m_energyPoints[i].SetActive(true);
        }
    }

    public void ChangeToDraggingVisual()
    {
        foreach (var visual in m_visualLayers)
        {
            visual.sortingLayerName = "Dragging Card";
        }
        foreach (var text in m_visualTexts)
        {
            text.sortingLayerID = SortingLayer.NameToID("Dragging Card");
        }
    }

    public void ResetToDefaultLayer()
    {
        foreach (var visual in m_visualLayers)
        {
            visual.sortingLayerName = "Default";
        }
        foreach (var text in m_visualTexts)
        {
            text.sortingLayerID = SortingLayer.NameToID("Default");
        }
    }
}
