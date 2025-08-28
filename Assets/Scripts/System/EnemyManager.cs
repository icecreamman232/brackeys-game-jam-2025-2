using SGGames.Scripts.Core;
using UnityEngine;

namespace SGGames.Scripts.System
{
    public class EnemyManager : MonoBehaviour, IBootStrap, IGameService
    {
        [SerializeField] private EnemyController m_currentEnemy;

        public void CreateEnemy(EnemyData data)
        {
            m_currentEnemy = Instantiate(data.EnemyPrefab, this.transform);
            m_currentEnemy.Health.OnDeath = OnEnemyDeath;
        }

        public void Install()
        {
            ServiceLocator.RegisterService<EnemyManager>(this);
        }

        public void Uninstall()
        {
            m_currentEnemy = null;
        }

        private void OnEnemyDeath()
        {
            m_currentEnemy.Health.OnDeath = null;
            Destroy(m_currentEnemy.gameObject);
            m_currentEnemy = null;
            var levelManager = ServiceLocator.GetService<LevelManager>();
            levelManager.LoadNextLevel();
        }
    }
}
