using SGGames.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_verstionText;
    [SerializeField] private ButtonController m_playButton;

    private void Awake()
    {
        m_playButton.OnClickAction = LoadGameplay;
        m_verstionText.text = $"v{Application.version}";   
    }

    private void LoadGameplay()
    {
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}
