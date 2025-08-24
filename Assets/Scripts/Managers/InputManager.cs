using System;
using SGGames.Scripts.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SGGames.Scripts.Managers
{
    public class InputManager : MonoBehaviour, IGameService
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private bool m_isActivated = true;
        private InputAction m_moveAction;
        private InputAction m_attackAction;

        public Action<Vector2> OnMoveInputCallback;
        public Action<Vector2> WorldMousePosition;
        public Action OnAttackInputCallback;

        private void Awake()
        {
            ServiceLocator.RegisterService<InputManager>(this);
        }

        private void Start()
        {
            m_moveAction = InputSystem.actions.FindAction("Move");
            m_isActivated = true;
        }
        
        private void Update()
        {
            if (!m_isActivated) return;
            OnMoveInputCallback?.Invoke(m_moveAction.ReadValue<Vector2>());
            WorldMousePosition?.Invoke(ComputeWorldMousePosition());
        }
        
        
        private Vector3 ComputeWorldMousePosition()
        {
            var mousePos = Input.mousePosition;
            mousePos = m_camera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            return mousePos;
        }
        
        private void AttackActionOnPerformed(InputAction.CallbackContext context)
        {
            OnAttackInputCallback?.Invoke();
        }
    }
}
