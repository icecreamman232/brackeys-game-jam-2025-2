using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroCanvas : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private Image m_enemyIcon;
    [SerializeField] private TextMeshProUGUI m_introText;
    [SerializeField] private Image m_blackBackground;

    private readonly int TRIGGER_INTRO = Animator.StringToHash("Trigger_Intro");
    
    public void PlayIntro(Sprite enemyIcon, string enemyName)
    {
        m_enemyIcon.sprite = enemyIcon;
        m_introText.text = $"{enemyName} just pooped!";
        m_animator.SetTrigger(TRIGGER_INTRO);
    }
}
