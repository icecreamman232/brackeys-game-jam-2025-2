using SGGames.Scripts.Core;
using SGGames.Scripts.Managers;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private Animator m_animator;
    private readonly int BOOL_IS_RUNNING = Animator.StringToHash("Is_Running");
    
    private void Start()
    {
        var inputManager = ServiceLocator.GetService<InputManager>();
        inputManager.OnMoveInputCallback += OnUpdateMovementInput;
    }

    private void OnDestroy()
    {
        var inputManager = ServiceLocator.GetService<InputManager>();
        inputManager.OnMoveInputCallback -= OnUpdateMovementInput;
    }

    private void OnUpdateMovementInput(Vector2 input)
    {
        transform.Translate(input * (m_speed * Time.deltaTime));
        m_animator.SetBool(BOOL_IS_RUNNING, input.magnitude > 0.1f);
    }
}
