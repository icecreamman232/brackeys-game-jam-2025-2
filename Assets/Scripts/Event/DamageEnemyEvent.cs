using SGGames.Scripts.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Enemy Event", menuName = "SGGames/Events/Damage Enemy")]
public class DamageEnemyEvent : ScriptableEvent<DamageEnemyInfo>
{
    
}

public class DamageEnemyInfo
{
    public int Damage;
}
