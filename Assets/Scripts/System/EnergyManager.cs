using SGGames.Scripts.Core;
using UnityEngine;

public class EnergyManager : MonoBehaviour, IGameService, IBootStrap
{
    [SerializeField] private int m_maxEnergy;
    [SerializeField] private int m_energyRemaining;
    [SerializeField] private SelectCardEvent m_selectCardEvent;
    [SerializeField] private EnergyHUDEvent m_energyHUDEvent;
    
    private EnergyHUDInfo m_energyHUDInfo;
    
    public bool CanSelectedThisCard(int energyCost)
    {
        return true;
        return m_energyRemaining > 0;
    }

    public void Reset()
    {
        m_energyRemaining = m_maxEnergy;
        m_energyHUDInfo.CurrentEnergy = m_energyRemaining;
        m_energyHUDInfo.MaxEnergy = m_maxEnergy;
        m_energyHUDEvent.Raise(m_energyHUDInfo);
    }

    public void RemoveEnergy(int energyCost)
    {
        m_energyRemaining -= energyCost;
        
        m_energyHUDInfo.CurrentEnergy = m_energyRemaining;
        m_energyHUDInfo.MaxEnergy = m_maxEnergy;
        m_energyHUDEvent.Raise(m_energyHUDInfo);
    }
    
    public void AddEnergy(int energyRecovered)
    {
        m_energyRemaining += energyRecovered;
        
        m_energyHUDInfo.CurrentEnergy = m_energyRemaining;
        m_energyHUDInfo.MaxEnergy = m_maxEnergy;
        m_energyHUDEvent.Raise(m_energyHUDInfo);
    }

    public void Install()
    {
        m_energyHUDInfo = new EnergyHUDInfo();
        ServiceLocator.RegisterService<EnergyManager>(this);
        m_energyRemaining = m_maxEnergy;
        m_energyHUDInfo.CurrentEnergy = m_energyRemaining;
        m_energyHUDInfo.MaxEnergy = m_maxEnergy;
        m_energyHUDEvent.Raise(m_energyHUDInfo);
        m_selectCardEvent.AddListener(OnCardSelected);
    }

    public void Uninstall()
    {
        ServiceLocator.UnregisterService<EnergyManager>();
        m_selectCardEvent.RemoveListener(OnCardSelected);
    }
    
    private void OnCardSelected(SelectCardEventData selectCardEventData)
    {
        if (selectCardEventData.IsSelected)
        {
            RemoveEnergy(selectCardEventData.EnergyCost);
        }
        else
        {
            AddEnergy(selectCardEventData.EnergyCost);    
        }
    }
}
