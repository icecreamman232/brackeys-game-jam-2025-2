using System;
using SGGames.Scripts.Managers;
using SGGames.Scripts.System;
using UnityEngine;

public class ItemBehavior : MonoBehaviour, IItem
{
    [SerializeField] protected int m_itemIndex;
    [SerializeField] protected ItemData m_itemData;
    [SerializeField] private BoxCollider2D m_itemCollider;

    private bool m_isDragging;
    private bool m_canClick = true;
    private Camera m_mainCamera;
    private Vector3 m_startDraggingPosition;
    private ItemBehavior m_overlappedItem;
    private bool m_canSwapWithOtherItem;
    
    // Add these new fields for drag detection
    private Vector3 m_mouseDownPosition;
    private float m_mouseDownTime;
    private bool m_dragIntentDetected = false;
    
    // Drag detection thresholds
    private const float k_DragDistanceThreshold = 0.5f; // Minimum distance to start dragging
    private const float k_DragTimeThreshold = 0.2f;     // Minimum time before drag can start

    protected (MultiplierType type, float value) DefaultItemValue => (m_itemData.MultiplierType,
        m_itemData.MultiplierType == MultiplierType.Multiply ? 1.0f : 0.0f);
    
    public ItemData ItemData => m_itemData;
    
    public int ItemIndex
    {
        get => m_itemIndex;
        set => m_itemIndex = value;
    }

    public BoxCollider2D ItemCollider => m_itemCollider;

    // Actions that ItemManager will hook into
    public Func<ItemBehavior, ItemBehavior> IsOverlappedOnItem;
    public Action<ItemBehavior, ItemBehavior> SwapItemsAction;
    public Action<ItemBehavior> ShowItemDescriptionAction;
    public Action<ItemBehavior> HideItemDescriptionAction;
    
    private void Awake()
    {
        m_mainCamera = Camera.main;
        
        // Get or add BoxCollider2D if not assigned
        if (m_itemCollider == null)
        {
            m_itemCollider = GetComponent<BoxCollider2D>();
            if (m_itemCollider == null)
            {
                m_itemCollider = gameObject.AddComponent<BoxCollider2D>();
            }
        }
    }

    private void Update()
    {
        if (m_isDragging)
        {
            var newPos = GetWorldMousePosition();
            transform.position = newPos;
        }
    }

    public void PlayTriggerAnimation()
    {
        var originalLocal = transform.localPosition;
        transform.LeanMoveLocalX(originalLocal.x - 0.15f, 0.1f)
            .setEase(LeanTweenType.punch)
            .setOnComplete(() =>
            {
                transform.LeanMoveLocalX(originalLocal.x + 0.15f, 0.1f)
                    .setEase(LeanTweenType.punch)
                    .setOnComplete(() =>
                    {
                        transform.localPosition = originalLocal;
                    });
            });
    }
    
    public virtual (MultiplierType type, float value) Use(CardManager cardManager)
    {
        return (m_itemData.MultiplierType, m_itemData.MultiplierValue);
    }

    /// <summary>
    /// Set current position as start dragging position.
    /// </summary>
    public void SetItemPosition(Vector3 position)
    {
        m_startDraggingPosition = position;
    }

    private void OnMouseDrag()
    {
        if (!InputManager.IsActivated) return;
        if (!m_canClick) return;
        
        // Check if we should start dragging
        if (!m_dragIntentDetected)
        {
            float timeSinceMouseDown = Time.time - m_mouseDownTime;
            float distanceFromStart = Vector3.Distance(GetWorldMousePosition(), m_mouseDownPosition);
        
            // Only start dragging if we've moved far enough OR held long enough
            if (distanceFromStart > k_DragDistanceThreshold || timeSinceMouseDown > k_DragTimeThreshold)
            {
                m_dragIntentDetected = true;
                m_isDragging = true;
            }
            else
            {
                return; // Don't process drag yet
            }
        }

        // Only process drag logic if drag intent is confirmed
        if (m_isDragging)
        {
            var overlappedItem = IsOverlappedOnItem?.Invoke(this);
            if (overlappedItem != null)
            {
                m_canSwapWithOtherItem = true;
                m_overlappedItem = overlappedItem;
            }
            else
            {
                m_canSwapWithOtherItem = false;
                m_overlappedItem = null;
            }
        }
    }

    private void OnMouseDown()
    {
        if (!InputManager.IsActivated) return;
        if (!m_canClick) return;

        // Store initial mouse position and time
        m_mouseDownPosition = GetWorldMousePosition();
        m_mouseDownTime = Time.time;
        m_dragIntentDetected = false;
    }

    private void OnMouseUp()
    {
        if (!InputManager.IsActivated) return;
        // If drag was detected, handle drag end
        if (m_dragIntentDetected)
        {
            HandleDragEnd();
        }

        // Reset drag detection state
        m_dragIntentDetected = false;
        m_isDragging = false;
    }

    private void HandleDragEnd()
    {
        if (m_canSwapWithOtherItem && m_overlappedItem != null)
        {
            SwapItemsAction?.Invoke(this, m_overlappedItem);
        }
        else
        {
            // Snap back to original position
            transform.position = m_startDraggingPosition;
        }

        m_canSwapWithOtherItem = false;
        m_overlappedItem = null;
    }

    private Vector3 GetWorldMousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos = m_mainCamera.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;
        return mousePos;
    }

    private void OnMouseEnter()
    {
        if (!InputManager.IsActivated) return;
        if (!m_isDragging) // Only scale if not dragging
        {
            transform.LeanScale(Vector3.one * 1.2f, 0.1f)
                .setOnComplete(() =>
                {
                    ShowItemDescriptionAction?.Invoke(this);
                });
        }
    }

    private void OnMouseExit()
    {
        if (!InputManager.IsActivated) return;
        if (!m_isDragging) // Only reset scale if not dragging
        {
            LeanTween.cancel(this.gameObject,false);
            transform.localScale = Vector3.one;
            HideItemDescriptionAction?.Invoke(this);
        }
    }

    public void TweenItemToPosition(Vector3 position, Action onFinish)
    {
        m_canClick = false;
        transform.LeanMove(position, 0.2f)
            .setEase(LeanTweenType.easeOutCirc)
            .setOnComplete(() =>
            {
                transform.position = position;
                onFinish?.Invoke();
                m_canClick = true;
            });
    }
}
