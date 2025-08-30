using SGGames.Scripts.Core;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IBootStrap, IGameService
{
    [SerializeField] private int m_finalScore;
    [SerializeField] private int m_score;
    [SerializeField] private float m_multiplier;
    [SerializeField] private DamageEnemyEvent m_damageEnemyEvent;

    private DamageEnemyInfo m_damageEnemyInfo;
    /// <summary>
    /// Score that is in counting process not the final score that has been multiplied.
    /// </summary>
    public int Score => m_score;
    public int FinalScore => m_finalScore;
    
    public void AddScoresFromCard(int score)
    {
        m_score += score;
    }

    public void AddMultiplier(float multiplier)
    {
        m_multiplier += multiplier;
    }

    public void ApplyEnergyDrain(int currentEnergy, int currentMaxEnergy)
    {
        var percent = (float) Mathf.Abs(currentEnergy) / currentMaxEnergy;
        var reduceMultiplier = 0f;
        
        if (percent < 0.25f)
        {
            reduceMultiplier = 0.75f;
        }
        else if (percent > 0.25f && percent < 1f)
        {
            reduceMultiplier = 0.5f;
        }
        else
        {
            reduceMultiplier = 0.25f;
        }
        var afterScore = m_finalScore * reduceMultiplier;
        m_finalScore = Mathf.RoundToInt(afterScore);
    }

    public void CalculateFinalScore()
    {
        m_finalScore = Mathf.RoundToInt(m_score * m_multiplier);
    }
    
    public void FinishScoreCounting()
    {
        //Send damage value to enemy
        m_damageEnemyInfo.Damage = m_finalScore;
        m_damageEnemyEvent?.Raise(m_damageEnemyInfo);
        
        //Reset counting score
        m_score = 0;
    }

    public void Install()
    {
        m_damageEnemyInfo = new DamageEnemyInfo();
        ServiceLocator.RegisterService<ScoreManager>(this);
    }

    public void Uninstall()
    {
        
    }
}
