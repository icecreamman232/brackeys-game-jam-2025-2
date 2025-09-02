using SGGames.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform m_titleText;
    [SerializeField] private TextMeshProUGUI m_verstionText;
    [SerializeField] private ButtonController m_playButton;
    [SerializeField] private BackgroundPatternScroll m_backgroundPatternScroll;
    [SerializeField] private BootStrapHandler m_bootStrap;
    
    private void Awake()
    {
        m_playButton.OnClickAction = LoadGameplay;
        m_verstionText.text = $"v{Application.version}";
        m_titleText.LeanRotateZ(5, 2)
            .setLoopPingPong();
        m_backgroundPatternScroll.Run();
    }

    private void LoadGameplay()
    {
        m_bootStrap.UninstallBootStrap();
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}
