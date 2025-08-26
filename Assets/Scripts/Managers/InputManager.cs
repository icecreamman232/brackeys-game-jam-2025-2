using System;
using SGGames.Scripts.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SGGames.Scripts.Managers
{
    public class InputManager : MonoBehaviour, IGameService, IBootStrap
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private bool m_isActivated = true;
        private InputAction m_moveAction;
        private InputAction m_attackAction;

        public Action<Vector2> OnMoveInputCallback;
        public Action<Vector2> WorldMousePosition;
        public Action OnAttackInputCallback;
        
        private void Update()
        {
            if (!m_isActivated) return;
            OnMoveInputCallback?.Invoke(m_moveAction.ReadValue<Vector2>());
            WorldMousePosition?.Invoke(ComputeWorldMousePosition());
        }
        
        public void Install()
        {
            ServiceLocator.RegisterService<InputManager>(this);
            m_moveAction = InputSystem.actions.FindAction("Move");
            m_isActivated = true;
        }

        public void Uninstall()
        {
            m_isActivated = false;
            ServiceLocator.UnregisterService<InputManager>();
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
