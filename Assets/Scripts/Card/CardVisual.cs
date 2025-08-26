using SGGames.Scripts.Data;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_atkPointText;
    [SerializeField] private TextMeshPro m_cardName;
    [SerializeField] private TextMeshPro m_cardDesc;
    [SerializeField] private SpriteRenderer m_cardIcon;

    public void ChangeCardVisual(CardData data)
    {
        m_atkPointText.text = data.Info.AttackPoint.ToString();
        m_cardName.text = data.Name;
        m_cardDesc.text = data.Description;
        m_cardIcon.sprite = data.Icon;
    }
}
