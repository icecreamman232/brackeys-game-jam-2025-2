using System;
using SGGames.Scripts.Data;
using SGGames.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace SGGames.Scripts.Card
{
    public enum CardState
    {
        InPile,
        InHand,
        InDiscard,
    }
    public class CardBehavior : MonoBehaviour
    {
        [SerializeField] private CardData m_cardData;
        [SerializeField] private CardVisual m_cardVisual;
        [SerializeField] private CardState m_cardState = CardState.InPile;
        [SerializeField] private int m_cardIndex;
        [SerializeField] private int m_atkPoint;
        [SerializeField] private SelectCardEvent m_selectCardEvent;
        [SerializeField] private TextMeshPro m_scoreDisplayer;
        [SerializeField] private BoxCollider2D m_cardCollider;
        [SerializeField] protected bool m_isSelected;
        private bool m_canSwawpWithOtherCard;
        private bool m_isDragging;
        protected bool m_canClick = true;
        private const float k_SelectCardYOffset = -2f;
        private const float k_DeselectOffset = -2.5f;
        private const float k_MoveCardTweenDuration = 0.2f;
        private Camera m_mainCamera;
        private Vector3 m_startDraggingPosition;
        private CardBehavior m_overlappedCard;
        private string m_originalName;
        
        // Add these new fields for drag detection
        private Vector3 m_mouseDownPosition;
        private float m_mouseDownTime;
        private bool m_dragIntentDetected = false;
        
        // Drag detection thresholds
        private const float k_DragDistanceThreshold = 0.5f; // Minimum distance to start dragging
        private const float k_DragTimeThreshold = 0.2f;     // Minimum time before drag can start

        public CardData CardData => m_cardData;
        public CardState CardState => m_cardState;
        public int CardIndex => m_cardIndex;
        public bool IsSelected => m_isSelected;
        
        public void SetCardIndex(int index) => m_cardIndex = index;

        public void SetName()
        {
            this.gameObject.name = $"{m_originalName} Index {m_cardIndex}";
        }
        
        public void SetData(CardData cardData)
        {
            m_cardData = cardData;
            m_atkPoint = m_cardData.Info.AttackPoint;
            m_cardVisual.ChangeCardVisual(m_cardData);
        }

        public int AttackPts => m_atkPoint;
        public BoxCollider2D CardCollider => m_cardCollider;
        
        public Func<CardBehavior, CardBehavior> IsOverlappedOnCard;
        public Func<int, bool> CanBeSelected;
        public Action<int> UseEnergyAction;
        public Action<CardBehavior,CardBehavior> SwapCardsAction;
        public Action SelectAction;
        public Action DeselectAction;

        private void Awake()
        {
            m_mainCamera = Camera.main;
            m_originalName = gameObject.name;
        }

        private void Update()
        {
            if (m_isDragging)
            {
                var newPos = GetWorldMousePosition();
                newPos.y -= 0.75f;
                transform.position = newPos;
            }
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
                var overlappedCard = IsOverlappedOnCard?.Invoke(this);
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

        private void OnMouseDown()
        {
            if (!InputManager.IsActivated) return;
            if (!m_canClick) return;

            // Store initial mouse position and time
            m_mouseDownPosition = GetWorldMousePosition();
            m_mouseDownTime = Time.time;
            m_dragIntentDetected = false;

            // Don't immediately select/deselect - wait to see if it's a drag
        }

        private void OnMouseUp()
        {
            if (!InputManager.IsActivated) return;
            // If no drag was detected, treat it as a click
            if (!m_dragIntentDetected)
            {
                HandleClick();
            }
            else
            {
                HandleDragEnd();
            }

            // Reset drag detection state
            m_dragIntentDetected = false;
            m_isDragging = false;
        }

        private void HandleClick()
        {
            // Original click behavior
            if (!m_isSelected)
            {
                if (!CanBeSelected.Invoke(m_cardData.Info.EnergyCost))
                {
                    return;
                }
                OnSelectTween();
            }
            else
            {
                OnDeselectTween();
            }
        }

        private void HandleDragEnd()
        {
            // Original drag end behavior
            if (m_canSwawpWithOtherCard)
            {
                SwapCardsAction?.Invoke(this, m_overlappedCard);
            }
            else
            {
                transform.position = m_startDraggingPosition;
            }

            m_isSelected = false;
            m_canSwawpWithOtherCard = false;
            m_overlappedCard = null;
        }

        public void ResetSelection()
        {
            m_canClick = true;
            m_isSelected = false;
            var currentLocal = transform.localPosition;
            currentLocal.y = k_DeselectOffset;
            transform.localPosition = currentLocal;
        }
        
        public void ChangeCardState(CardState state)
        {
            m_cardState = state;
        }

        /// <summary>
        /// Set current position as start dragging position.
        /// </summary>
        public void SetHandPosition(Vector3 position)
        {
            m_startDraggingPosition = position;
        }
        
        public virtual void OnSelect()
        {
            SelectAction?.Invoke();
            m_selectCardEvent.Raise(new SelectCardEventData
            {
                CardIndex = m_cardIndex,
                EnergyCost = m_cardData.Info.EnergyCost,
                IsSelected = true
            });
        }
        
        public virtual void OnDeselect()
        {
            DeselectAction?.Invoke();
            m_selectCardEvent.Raise(new SelectCardEventData
            {
                CardIndex = m_cardIndex,
                EnergyCost = m_cardData.Info.EnergyCost,
                IsSelected = false
            });
        }

        public void ShowAtkPointHUD()
        {
            m_scoreDisplayer.gameObject.SetActive(true);
            m_scoreDisplayer.text = $"+{m_atkPoint}";
        }

        public void HideAtkPointHUD()
        {
            m_scoreDisplayer.gameObject.SetActive(false);
        }

        public void TweenCardToPosition(Vector3 position, Action onFinish)
        {
            m_canClick = false;
            transform.LeanMove(position, k_MoveCardTweenDuration)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(() =>
                {
                    transform.position = position;
                    onFinish?.Invoke();
                    m_canClick = true;
                });
        }

        private Vector3 GetWorldMousePosition()
        {
            var mousePos = Input.mousePosition;
            mousePos = m_mainCamera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            return mousePos;
        }

        private void OnCompleteTween()
        {
            m_isSelected = !m_isSelected;
            m_canClick = true;
            if (m_isSelected)
            {
                OnSelect();
            }
            else
            {
                OnDeselect();
            }
        }

        private void OnSelectTween()
        {
            m_canClick = false;
            transform.LeanMoveLocalY(k_SelectCardYOffset, k_MoveCardTweenDuration).setEase(LeanTweenType.easeOutCirc).setOnComplete(OnCompleteTween);
        }
        
        private void OnDeselectTween()
        {
            m_canClick = false;
            transform.LeanMoveLocalY(k_DeselectOffset, k_MoveCardTweenDuration).setEase(LeanTweenType.easeOutCirc).setOnComplete(OnCompleteTween);
        }

        // private void OnDrawGizmos()
        // {
        //     //if (m_isDragging)
        //     {
        //         Gizmos.color = Color.red;
        //         var cardCenter = m_cardCollider.bounds.center;
        //         Gizmos.DrawCube(cardCenter, m_cardCollider.size);
        //     }
        // }
    }
}
