using SGGames.Scripts.Core;
using UnityEngine;

namespace SGGames.Scripts.System
{
    public class EnemyManager : MonoBehaviour, IBootStrap, IGameService
    {
        [SerializeField] private int m_maxAttempts;
        [SerializeField] private int m_attemptNumber;
        [SerializeField] private GameEvent m_gameEvent;
        
        private EnemyController m_currentEnemy;

        public void CreateEnemy(EnemyData data)
        {
            m_currentEnemy = Instantiate(data.EnemyPrefab, this.transform);
            m_currentEnemy.Health.OnDeath = OnEnemyDeath;
            m_currentEnemy.Health.OnTakeDamage = OnDamageEnemy;
            m_attemptNumber = m_maxAttempts;
        }

        public void Install()
        {
            ServiceLocator.RegisterService<EnemyManager>(this);
        }

        public void Uninstall()
        {
            m_currentEnemy = null;
        }

        private void OnDamageEnemy()
        {
            m_attemptNumber--;
            if (m_attemptNumber <= 0)
            {
                //LOSE GAME HERE
                m_gameEvent.Raise(GameEventType.Defeat);
            }
        }
        
        [ContextMenu("Kill Enemy")]
        private void OnEnemyDeath()
        {
            m_currentEnemy.Health.OnTakeDamage = null;
            m_currentEnemy.Health.OnDeath = null;
            Destroy(m_currentEnemy.gameObject);
            m_currentEnemy = null;
            m_gameEvent.Raise(GameEventType.Victory);
        }
    }
}
