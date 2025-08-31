using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using SGGames.Scripts.UI;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IBootStrap, IGameService
{
    [SerializeField] private GameObject m_panel;
    [SerializeField] private GameObject[] m_focusViewTutorials;
    [SerializeField] private GameObject[] m_tutorialTexts;
    [SerializeField] private ButtonController[] m_buttonControllers;
    
    private bool m_isTutorialCompleted;
    private string m_tutorialKey = "TutorialCompleted";
    

    public void StartTutorial()
    {
        if (m_isTutorialCompleted) return;
        
        InputManager.SetActive(false);
        
        m_panel.SetActive(true);
        
        for (int i = 0; i < m_buttonControllers.Length - 1; i++)
        {
            var currentIndex = i;
            var nextIndex = i + 1;
            m_buttonControllers[i].OnClickAction = () =>
            {
                m_tutorialTexts[currentIndex].SetActive(false);
                m_focusViewTutorials[currentIndex].SetActive(false);
                m_tutorialTexts[nextIndex].SetActive(true);
                m_focusViewTutorials[nextIndex].SetActive(true);
            };
        }

        m_buttonControllers[^1].OnClickAction = () =>
        {
            SaveTutorialCompleted();
            ExitTutorial();
        };
        
        m_tutorialTexts[0].SetActive(true);
        m_focusViewTutorials[0].SetActive(true);
    }

    private void SaveTutorialCompleted()
    {
        PlayerPrefs.SetInt(m_tutorialKey, 1);
        PlayerPrefs.Save();
    }

    private void ExitTutorial()
    {
        //Debug.Log("Exit Tutorial");
        SaveTutorialCompleted();
        m_panel.SetActive(false);
        m_tutorialTexts[^1].SetActive(false);
        m_focusViewTutorials[^1].SetActive(false);
        InputManager.SetActive(true);
    }

    [ContextMenu("Reset Tutorial")]
    private void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(m_tutorialKey);
        PlayerPrefs.Save();
    }

    public void Install()
    {
       m_isTutorialCompleted = PlayerPrefs.GetInt(m_tutorialKey, 0) == 1;
       ServiceLocator.RegisterService<TutorialManager>(this);
    }

    public void Uninstall()
    {
        ServiceLocator.UnregisterService<TutorialManager>();
    }
}
