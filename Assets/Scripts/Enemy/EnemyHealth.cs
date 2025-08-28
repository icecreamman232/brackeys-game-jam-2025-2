using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private DamageEnemyEvent m_damageEvent;
    [SerializeField] private EnemyHealthBarEvent m_healthBarEvent;
    [SerializeField] private int m_currentHealth;
    [SerializeField] private int m_maxHealth;

    private EnemyHealthBarEventData m_healthBarEventData;
    private bool m_canTakeHit = true;

    public Action OnDeath;

    public void Initialize()
    {
        m_currentHealth = m_maxHealth;
        m_healthBarEventData = new EnemyHealthBarEventData()
        {
            CurrentHealth = m_currentHealth,
            MaxHealth = m_maxHealth
        };
        m_healthBarEvent.Raise(m_healthBarEventData);
        m_damageEvent.AddListener(TakeDamage);
    }

    private void OnDestroy()
    {
        m_damageEvent.RemoveListener(TakeDamage);
    }

    private void TakeDamage(DamageEnemyInfo info)
    {
        if (!m_canTakeHit) return;
        
        m_currentHealth -= info.Damage;
        
        m_healthBarEventData.CurrentHealth = m_currentHealth;
        m_healthBarEventData.MaxHealth = m_maxHealth;
        m_healthBarEvent.Raise(m_healthBarEventData);
        
        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        m_canTakeHit = false;
        OnDeath?.Invoke();
    }
}
