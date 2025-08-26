using UnityEngine;

namespace SGGames.Scripts.System
{
    public class EnemyManager : MonoBehaviour, IBootStrap
    {
        [SerializeField] private EnemyController m_enemyPrefab;
        private EnemyController m_currentEnemy;

        private EnemyController CreateEnemy()
        {
            var enemy = Instantiate(m_enemyPrefab, this.transform);
            return enemy;
        }

        public void Install()
        {
            m_currentEnemy = CreateEnemy();
        }

        public void Uninstall()
        {
            m_currentEnemy = null;
        }
    }
}
