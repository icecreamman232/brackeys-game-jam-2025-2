using System;
using UnityEngine;

namespace SGGames.Scripts.Events
{
    public abstract class ScriptableEvent<T> : ScriptableObject
    {
        private Action<T> m_listener;
        public virtual void AddListener(Action<T> addListener) => m_listener += addListener;
        public virtual void RemoveListener(Action<T> removeListener) => m_listener -= removeListener;
        public virtual void Raise(T arg) => m_listener?.Invoke(arg);
        public virtual void RemoveAllListeners() => m_listener = null; 
    }
}

