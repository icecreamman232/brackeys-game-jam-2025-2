using SGGames.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScreenUI : MonoBehaviour
{
    [SerializeField] private WinGameUI m_winGameUI;
    [SerializeField] private LoseGameUI m_loseGameUI;
    [SerializeField] private GameEvent m_gameEvent;

    private void Awake()
    {
        m_gameEvent.AddListener(OnGameEventChanged);
        m_loseGameUI.OnLoadToMainMenuAction = LoadToMainMenu;
    }

    private void OnDestroy()
    {
        m_gameEvent.RemoveListener(OnGameEventChanged);
    }

    private void LoadToMainMenu()
    {
        ServiceLocator.GetService<BootStrapHandler>().UninstallBootStrap();
        ServiceLocator.ClearServices();
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }

    private void OnGameEventChanged(GameEventType eventType)
    {
        if (eventType == GameEventType.Victory)
        {
            m_winGameUI.ShowPanel();
        }
        else if (eventType == GameEventType.Defeat)
        {
            m_loseGameUI.ShowPanel();
        }
    }
}
