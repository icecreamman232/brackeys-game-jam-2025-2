using UnityEngine;

public class ScoreManager : MonoBehaviour, IBootStrap
{
    [SerializeField] private int m_finalScore;
    [SerializeField] private int m_score;
    [SerializeField] private float m_multiplier;
    [SerializeField] private DamageEnemyEvent m_damageEnemyEvent;

    private DamageEnemyInfo m_damageEnemyInfo;
    
    public int FinalScore => m_finalScore;
    
    public void AddScoresFromCard(int score)
    {
        m_score += score;
    }

    public void AddMultiplier(float multiplier)
    {
        m_multiplier += multiplier;
    }

    public void FinishScoreCounting()
    {
        m_finalScore = Mathf.RoundToInt(m_score * m_multiplier);
        
        //Send damage value to enemy
        m_damageEnemyInfo.Damage = m_finalScore;
        m_damageEnemyEvent?.Raise(m_damageEnemyInfo);
    }

    public void Install()
    {
        m_damageEnemyInfo = new DamageEnemyInfo();
    }

    public void Uninstall()
    {
        
    }
}
