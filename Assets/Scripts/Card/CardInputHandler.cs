using System;
using SGGames.Scripts.Managers;
using UnityEngine;

namespace SGGames.Scripts.Card
{
    public class CardInputHandler : MonoBehaviour
    {
        [SerializeField] private CardBehavior m_card;

        
        private bool m_isDragging;
        private bool m_dragIntentDetected;
        private float m_mouseDownTime;
        private Vector3 m_mouseDownPosition;
        private bool m_canSwawpWithOtherCard;
        private CardBehavior m_overlappedCard;

        private const float k_DragDistanceThreshold = 0.5f; // Minimum distance to start dragging
        private const float k_DragTimeThreshold = 0.2f;     // Minimum time before drag can start
        
        public bool CanClick { get; set; }
        public Vector3 StartDragPositon { get; set; }
        
        public bool CanSwap => m_canSwawpWithOtherCard;
        public CardBehavior OverlappedCard => m_overlappedCard;
        
        public Func<CardBehavior, CardBehavior> IsOverlappedOnCard;
        public event Action OnMouseDownOnCard;
        public event Action OnCardClicked;
        public event Action OnDragStarted;
        public event Action OnDragEnded;

        private void Update()
        {
            if (!m_isDragging) return;
            var newPos = InputManager.GetWorldMousePosition();
            newPos.y -= 0.75f;
            transform.position = newPos;
        }
        
        private void OnMouseDown()
        {
            if (!InputManager.IsActivated) return;
            if (!CanClick) return;
            
            OnMouseDownOnCard?.Invoke();
            // Store initial mouse position and time
            m_mouseDownPosition = InputManager.GetWorldMousePosition();
            m_mouseDownTime = Time.time;
            m_dragIntentDetected = false;
        }
        
        private void OnMouseDrag()
        {
            if (!InputManager.IsActivated) return;
            if (!CanClick) return;
            
            // Check if we should start dragging
            if (!m_dragIntentDetected)
            {
                if(ShouldStartDrag())
                {
                    m_dragIntentDetected = true;
                    m_isDragging = true;
                    OnDragStarted?.Invoke();
                }
                else
                {
                    return;
                }
            }

            if (m_isDragging)
            {
                var overlappedCard = IsOverlappedOnCard?.Invoke(m_card);
                if (overlappedCard != null)
                {
                    m_canSwawpWithOtherCard = true;
                    m_overlappedCard = overlappedCard;
                }
                else
                {
                    m_canSwawpWithOtherCard = false;
                    m_overlappedCard = null;
                }
            }
        }

        private void OnMouseUp()
        {
            if (!InputManager.IsActivated) return;
            if (!CanClick) return;
            
            // If no drag was detected, treat it as a click
            if (!m_dragIntentDetected)
            {
                OnCardClicked?.Invoke();
            }
            else
            {
                OnDragEnded?.Invoke();
            }

            ResetDragState();
        }
        
        private bool ShouldStartDrag()
        {
            float timeSinceMouseDown = Time.time - m_mouseDownTime;
            float distanceFromStart = Vector3.Distance(InputManager.GetWorldMousePosition(), m_mouseDownPosition);
            
            // Only start dragging if we've moved far enough OR held long enough
            return (distanceFromStart > k_DragDistanceThreshold || timeSinceMouseDown > k_DragTimeThreshold);
        }

        private void ResetDragState()
        {
            // Reset drag detection state
            m_dragIntentDetected = false;
            m_isDragging = false;
            m_canSwawpWithOtherCard = false;
            m_overlappedCard = null;
        }
    }
}
