using SGGames.Scripts.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Health Bar Event", menuName = "SGGames/Events/Enemy Health Bar")]
public class EnemyHealthBarEvent : ScriptableEvent<EnemyHealthBarEventData>
{
    
}

public class EnemyHealthBarEventData
{
    public int CurrentHealth;
    public int MaxHealth;
}
