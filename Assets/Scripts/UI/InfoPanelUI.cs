using TMPro;
using UnityEngine;

public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_handNumberText;
    [SerializeField] private TextMeshProUGUI m_discardNumberText;
    [SerializeField] private HandNumberEvent m_handNumberEvent;
    [SerializeField] private DiscardNumberEvent m_discardNumberEvent;
    
    private void OnDestroy()
    {
        m_handNumberEvent.RemoveListener(UpdateHandNumber);
        m_discardNumberEvent.RemoveListener(UpdateDiscardNumber);
    }

    public void Initialize()
    {
        m_handNumberEvent.AddListener(UpdateHandNumber);
        m_discardNumberEvent.AddListener(UpdateDiscardNumber);
    }

    private void UpdateDiscardNumber(int currentDiscardNumber)
    {
        m_discardNumberText.text = currentDiscardNumber.ToString();
    }
    
    private void UpdateHandNumber(int currentHandNumber)
    {
        m_handNumberText.text = currentHandNumber.ToString();
    }
}
