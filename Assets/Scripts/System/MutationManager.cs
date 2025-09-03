using SGGames.Scripts.Core;
using SGGames.Scripts.Data;
using SGGames.Scripts.System;
using UnityEngine;

namespace SGGames.Scripts.Mutation
{
    public class MutationManager : MonoBehaviour, IBootStrap
    {
        [SerializeField] private int m_numberEnergyStored;
        [SerializeField] private int m_maxEnergyStored;
        [SerializeField] private GameEvent m_gameEvent;
        
        public void Install()
        {
            m_gameEvent.AddListener(OnReceiveGameEvent);
        }

        public void Uninstall()
        {
            m_gameEvent.RemoveListener(OnReceiveGameEvent);
        }

        private void CreateMutation(CardElement element)
        {
            Debug.Log("Create mutation");
        }

        private void CheckForMutation()
        {
            Debug.Log("Check for mutation");
            var energyExcess = ServiceLocator.GetService<EnergyManager>().EnergyRemaining;
            m_numberEnergyStored += energyExcess;
            Debug.Log($"Number of energy stored:{m_numberEnergyStored}");
            if (m_numberEnergyStored >= m_maxEnergyStored)
            {
                var majorityElement = ServiceLocator.GetService<CardManager>().MajorityElement;
                CreateMutation(majorityElement);
            }
        }
        
        private void OnReceiveGameEvent(GameEventType eventType)
        {
            if (eventType == GameEventType.CheckMutation)
            {
                CheckForMutation();
            }
        }
    }
}
