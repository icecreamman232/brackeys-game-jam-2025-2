using TMPro;
using UnityEngine;

public class EnergyHUD : MonoBehaviour
{
    [SerializeField] private EnergyHUDEvent m_energyHUDEvent;
    [SerializeField] private TextMeshProUGUI m_energyText;

    private void Awake()
    {
        m_energyHUDEvent.AddListener(UpdateEnergyHUD);
    }

    private void OnDestroy()
    {
        m_energyHUDEvent.RemoveListener(UpdateEnergyHUD);
    }

    private void UpdateEnergyHUD(EnergyHUDInfo info)
    {
        m_energyText.text = $"{info.CurrentEnergy}/{info.MaxEnergy}";
    }
}