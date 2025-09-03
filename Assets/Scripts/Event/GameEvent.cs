using SGGames.Scripts.Events;
using UnityEngine;

public enum GameEventType
{
    Victory,
    Defeat,
    CheckMutation,
}


[CreateAssetMenu(fileName = "Game Event", menuName = "SGGames/Events/Game Event")]
public class GameEvent : ScriptableEvent<GameEventType>
{
    
}
