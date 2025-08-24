using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    [SerializeField] private Vector2 m_aimDirection;

    public Vector2 AimDirection => m_aimDirection;
    
    private void Start()
    {
        var inputManager = ServiceLocator.GetService<InputManager>();
        inputManager.WorldMousePosition += OnMousePositionUpdate;
    }

    private void OnDestroy()
    {
        var inputManager = ServiceLocator.GetService<InputManager>();
        inputManager.WorldMousePosition -= OnMousePositionUpdate;
    }

    private void OnMousePositionUpdate(Vector2 mousePosition)
    {
        m_aimDirection = (mousePosition - (Vector2)transform.position).normalized;
    }
}
