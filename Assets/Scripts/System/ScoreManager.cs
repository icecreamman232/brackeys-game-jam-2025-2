using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int m_score;
    
    public void AddScoresFromCards(List<int> scores)
    {
        foreach (var score in scores)
        {
            m_score += score;
        }
    }
}
