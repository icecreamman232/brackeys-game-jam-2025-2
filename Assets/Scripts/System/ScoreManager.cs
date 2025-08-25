using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int m_score;
    
    public void AddScoresFromCard(int score)
    {
        m_score += score;
    }

    public void FinishScoreCounting()
    {
        
    }
}
