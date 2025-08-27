using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "SGGames/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    public EnemyController EnemyPrefab;
    public Sprite EnemyIcon;
}
