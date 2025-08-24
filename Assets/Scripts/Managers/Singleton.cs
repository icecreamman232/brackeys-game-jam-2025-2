using UnityEngine;

namespace SGGames.Scripts.Core
{
    public class Singleton<T> : MonoBehaviour where T:Component
    {
        private static T m_instance = null;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindFirstObjectByType<T>();
                    if (m_instance == null)
                    {
                        GameObject newObj = new GameObject();
                        m_instance = newObj.AddComponent<T>();
                    }
                }

                return m_instance;
            }
        }

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (m_instance == null)
            {
                //if this is first instance, make it Singleton
                m_instance = this as T;
                m_instance.transform.parent = null;
            }
            else
            {
                //if an instance exists and we find another in scene, destroy it
                if (this != m_instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}

