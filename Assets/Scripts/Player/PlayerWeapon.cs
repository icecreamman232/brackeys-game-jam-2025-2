using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Transform m_weaponAttachment;
    [SerializeField] private Transform m_weapon;
    [SerializeField] private Transform m_model;
    [SerializeField] private PlayerAiming m_aiming;
    [SerializeField] private Animator m_animator;

    private readonly int TRIGGER_ATTACK = Animator.StringToHash("Trigger_Attack");
    private readonly int BOOL_IS_ATK_LEFT = Animator.StringToHash("Is_Attack_Left");
    
    private void Start()
    {
        var inputManager = ServiceLocator.GetService<InputManager>();
        inputManager.OnAttackInputCallback += OnAttackInputPressed;
    }

    private void OnDestroy()
    {
        var inputManager = ServiceLocator.GetService<InputManager>();
        inputManager.OnAttackInputCallback -= OnAttackInputPressed;
    }

    private void Update()
    {
        RotateWeapon();
    }
    
    private void OnAttackInputPressed()
    {
        m_animator.SetTrigger(TRIGGER_ATTACK);
        m_animator.SetBool(BOOL_IS_ATK_LEFT, m_aiming.AimDirection.x < 0);
    }

    private void RotateWeapon()
    {
        m_model.localScale = m_aiming.AimDirection.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;
        
        var angle = Mathf.Atan2(m_aiming.AimDirection.y, m_aiming.AimDirection.x) * Mathf.Rad2Deg;
        m_weaponAttachment.rotation = Quaternion.Euler(0, 0, angle);

        m_weapon.localScale = Vector3.one;
        
        if ((angle <= -90) || (angle >= 90))
        {
            m_weapon.localScale = new Vector3(1, -1, 1);
        }
    }
}
