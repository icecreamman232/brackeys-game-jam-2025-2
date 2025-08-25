using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int m_finalScore;
    [SerializeField] private int m_score;
    [SerializeField] private float m_multiplier;
    
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
    }
}
