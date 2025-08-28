using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyHealth m_enemyHealth;

    public EnemyHealth Health => m_enemyHealth;
    
    private void Start()
    {
        m_enemyHealth.Initialize();
    }
}
