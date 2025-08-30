using SGGames.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private ButtonController m_playButton;

    private void Awake()
    {
        m_playButton.OnClickAction = LoadGameplay;
    }

    private void LoadGameplay()
    {
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}
