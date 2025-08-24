using System;
using System.Collections.Generic;

namespace SGGames.Scripts.Core
{
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        private static Dictionary<Type,IGameService> m_services = new Dictionary<Type,IGameService>();

        public static void RegisterService<T>(IGameService service) where T : IGameService
        {
            if (m_services.ContainsKey(typeof(T)))
            {
                return;
            }
            m_services.Add(typeof(T),service);
        }

        public static T GetService<T>() where T : IGameService
        {
            if (m_services.ContainsKey(typeof(T)))
            {
                return (T)m_services[typeof(T)];
            }
            throw new KeyNotFoundException("Service not registered");
        }
        
        public static bool HasService<T>() where T : IGameService
        {
            return m_services.ContainsKey(typeof(T));
        }

        public static void UnregisterService<T>()
        {
            if (m_services.ContainsKey(typeof(T)))
            {
                m_services.Remove(typeof(T));
            }
        }
        
        public static void ClearServices()
        {
            // Clear all services
            m_services.Clear();
        }
    }
}

