using System;
using SGGames.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image m_healthBar;
    [SerializeField] private Image m_enemyIcon;
    [SerializeField] private EnemyHealthBarEvent m_enemyHealthBarEvent;

    private void Start()
    {
        m_enemyHealthBarEvent.AddListener(UpdateHealthBar);
    }

    private void OnDestroy()
    {
        m_enemyHealthBarEvent.RemoveListener(UpdateHealthBar);
    }

    private void UpdateHealthBar(EnemyHealthBarEventData data)
    {
        m_healthBar.fillAmount = 1 - MathHelpers.Remap(data.CurrentHealth, 0, data.MaxHealth, 0, 1);
        m_canvasGroup.alpha = m_healthBar.fillAmount > 0 ? 1 : 0;
    }
}
