using System.Collections.Generic;
using UnityEngine;

namespace SGGames.Scripts.Core
{
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private bool m_autoCreateParentForPool;
        [SerializeField] private GameObject m_parent;
        [SerializeField] private bool m_isSharePool;
        [SerializeField] private bool m_pooledObjectActiveByDefault;
        [SerializeField] private GameObject m_objectToPool;
        [SerializeField] private int m_poolSize;

        private List<GameObject> m_pool;
        private GameObject m_autoCreatedPoolParent;

        public List<GameObject> CurrentPool => m_pool;
        
        protected virtual void Awake()
        {
            CreatePool();
        }
        
        private void CreatePool()
        {
            if (m_objectToPool == null) return;
            
            if (m_pool == null)
            {
                m_pool = new List<GameObject>();
            }

            if (m_autoCreateParentForPool)
            {
                m_autoCreatedPoolParent = new GameObject(m_objectToPool.name + " Parent");
            }
            

            //Find existing pool with same name
            if (m_isSharePool)
            {
                var pools = FindObjectsByType<ObjectPooler>(FindObjectsSortMode.None);
                for (int i = 0; i < pools.Length; i++)
                {
                    if(pools[i].GetInstanceID() == this.GetInstanceID()) continue;
                    if (pools[i].m_objectToPool.name == m_objectToPool.name && pools[i].m_objectToPool!=null)
                    {
                        m_pool = pools[i].CurrentPool;
                        return;
                    }
                }
            }
            
            for (int i = 0; i < m_poolSize; i++)
            {
                var pooledObject = Instantiate(m_objectToPool, m_autoCreateParentForPool ? m_autoCreatedPoolParent.transform : m_parent.transform);
                var currentName = pooledObject.name;
                currentName += $"({i})";
                pooledObject.name = currentName;
                pooledObject.SetActive(m_pooledObjectActiveByDefault);
                m_pool.Add(pooledObject);
            }
        }

        public GameObject GetPooledGameObject()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                if (!m_pool[i].activeInHierarchy)
                {
                    m_pool[i].SetActive(true);
                    return m_pool[i];
                }
            }
            
            return null;
        }

        public void CleanUp()
        {
            if (m_autoCreatedPoolParent != null)
            {
                Destroy(m_autoCreatedPoolParent);
                return;
            }
            
            
            for (int i = 0; i < m_pool.Count; i++)
            {
                Destroy(m_pool[i].gameObject);
            }
        }
    }
}

