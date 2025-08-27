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
        }

        public void Install()
        {
            ServiceLocator.RegisterService<EnemyManager>(this);
        }

        public void Uninstall()
        {
            m_currentEnemy = null;
        }
    }
}
