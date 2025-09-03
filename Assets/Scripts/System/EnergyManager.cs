using SGGames.Scripts.Core;
using UnityEngine;

public class EnergyManager : MonoBehaviour, IGameService, IBootStrap
{
    [SerializeField] private int m_maxEnergy;
    [SerializeField] private int m_energyRemaining;
    [SerializeField] private SelectCardEvent m_selectCardEvent;
    [SerializeField] private EnergyHUDEvent m_energyHUDEvent;
    [SerializeField] private GameEvent m_gameEvent;
    
    private EnergyHUDInfo m_energyHUDInfo;
    
    public (int current, int max) EnergyInfo => (m_energyRemaining, m_maxEnergy);
    
    public bool IsEnergyDrain => m_energyRemaining < 0;
    public int EnergyRemaining => m_energyRemaining;

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
        m_gameEvent.AddListener(OnReceiveGameEvent);
    }

    public void Uninstall()
    {
        ServiceLocator.UnregisterService<EnergyManager>();
        m_selectCardEvent.RemoveListener(OnCardSelected);
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
    }
    
    private void OnReceiveGameEvent(GameEventType eventType)
    {
        if (eventType == GameEventType.CheckMutation)
        {
            //Calculate excess energy for mutation
        }
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
