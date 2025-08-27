using System.Collections;
using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using SGGames.Scripts.System;
using UnityEngine;

public class LevelManager : MonoBehaviour, IGameService, IBootStrap
{
    [SerializeField] private IntroCanvas m_introCanvas;
    [SerializeField] private EnemyData[] m_enemies;
    
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
        enemyManager.CreateEnemy(m_enemies[0]);
    }
    
    private IEnumerator PlayIntroAnimation()
    {
        m_introCanvas.PlayIntro(m_enemies[0].EnemyIcon, m_enemies[0].EnemyName);
        yield return new WaitForSeconds(1.5f);
    }

    public void LoadNextLevel()
    {
        
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