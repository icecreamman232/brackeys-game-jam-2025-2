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
        CreateEnemy();
        yield return StartCoroutine(PlayIntroAnimation());
        InputManager.SetActive(true);
    }

    private void CreateEnemy()
    {
        var enemyManager = ServiceLocator.GetService<EnemyManager>();
        enemyManager.CreateEnemy(m_enemies[m_currentLevel]);
    }
    
    private IEnumerator PlayIntroAnimation()
    {
        m_introCanvas.PlayIntro(m_enemies[m_currentLevel].EnemyIcon, m_enemies[0].EnemyName);
        yield return new WaitForSeconds(k_IntroAnimDuration);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(OnLoadNextLevel());
    }

    private IEnumerator OnLoadNextLevel()
    {
        InputManager.SetActive(false);
        CreateEnemy();
        yield return StartCoroutine(PlayIntroAnimation());
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