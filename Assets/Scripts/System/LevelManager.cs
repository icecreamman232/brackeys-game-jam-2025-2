using System.Collections;
using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using SGGames.Scripts.System;
using UnityEngine;

public class LevelManager : MonoBehaviour, IGameService, IBootStrap
{
    [SerializeField] private IntroCanvas m_introCanvas;
    [SerializeField] private EnemyData[] m_enemies;
    private int m_currentLevel = 0;
    
    private const float k_IntroAnimDuration = 1.833f;
    
    public void LoadFirstLevel()
    {
        StartCoroutine(OnLoadFirstLevel());
    }

    private IEnumerator OnLoadFirstLevel()
    {
        InputManager.SetActive(false);
        if (m_currentLevel >= m_enemies.Length)
        {
            m_currentLevel = m_enemies.Length - 1;       
        }
        CreateEnemy();
        yield return StartCoroutine(PlayIntroAnimation());
        ServiceLocator.GetService<CardManager>().DealFirstHands();
        InputManager.SetActive(true);
    }

    private void CreateEnemy()
    {
        var enemyManager = ServiceLocator.GetService<EnemyManager>();
        enemyManager.CreateEnemy(m_enemies[m_currentLevel]);
    }
    
    private IEnumerator PlayIntroAnimation()
    {
        m_introCanvas.PlayIntro(m_enemies[m_currentLevel].EnemyIcon, m_enemies[m_currentLevel].EnemyName);
        yield return new WaitForSeconds(k_IntroAnimDuration);
    }

    public void LoadNextLevel()
    {
        Debug.Log("Load next level");
        StartCoroutine(OnLoadNextLevel());
    }

    private IEnumerator OnLoadNextLevel()
    {
        InputManager.SetActive(false);
        m_currentLevel++;
        if (m_currentLevel >= m_enemies.Length)
        {
            m_currentLevel = m_enemies.Length - 1;      
        }
        CreateEnemy();
        yield return StartCoroutine(PlayIntroAnimation());
        ServiceLocator.GetService<CardManager>().DealFirstHands();
        InputManager.SetActive(true);
    }

    public void Install()
    {
        ServiceLocator.RegisterService<LevelManager>(this);
        LoadFirstLevel();
    }

    public void Uninstall()
    {
        ServiceLocator.UnregisterService<LevelManager>();
    }
}