using System;
using SGGames.Scripts.Events;
using UnityEngine;

namespace SGGames.Scripts.Event
{
    [CreateAssetMenu(fileName = "Count Score Event", menuName = "SGGames/Events/Count Score")]
    public class CountScoreEvent : ScriptableEvent<CountScoreEventData> { }

    public enum EntityType
    {
        Card,
        Artifact,
    }
    
    [Serializable]
    public class CountScoreEventData
    {
        public int Score;
        public int PositionIndex;
        public EntityType EntityType;
    }
}
