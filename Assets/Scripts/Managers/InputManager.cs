using System;
using SGGames.Scripts.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SGGames.Scripts.Managers
{
    public class InputManager : MonoBehaviour, IGameService, IBootStrap
    {
        private static Camera m_camera;
        private InputAction m_moveAction;
        private InputAction m_attackAction;

        public Action<Vector2> OnMoveInputCallback;
        public Action<Vector2> WorldMousePosition;
        public Action OnAttackInputCallback;

        public static bool IsActivated;

        public static void SetActive(bool isActive)
        {
            IsActivated = isActive;
            if (isActive)
            {
                InputSystem.actions.Enable();
            }
            else
            {
                InputSystem.actions.Disable();
            }
            //Debug.Log("InputManager is " + (isActive ? "activated" : "deactivated"));
        }
        
        public static Vector3 GetWorldMousePosition()
        {
            var mousePos = Input.mousePosition;
            mousePos = m_camera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            return mousePos;
        }
        
        private void Update()
        {
            if (!IsActivated) return;
            OnMoveInputCallback?.Invoke(m_moveAction.ReadValue<Vector2>());
            WorldMousePosition?.Invoke(ComputeWorldMousePosition());
        }
        
        public void Install()
        {
            m_camera = Camera.main;
            ServiceLocator.RegisterService<InputManager>(this);
            m_moveAction = InputSystem.actions.FindAction("Move");
            IsActivated = true;
        }

        public void Uninstall()
        {
            IsActivated = false;
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
