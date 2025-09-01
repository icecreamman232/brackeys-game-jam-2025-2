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
        [SerializeField] private PlaySFXEvent m_playSFXEvent;
        [SerializeField] private CardVisual m_cardVisual;
        [SerializeField] private CardState m_cardState = CardState.InPile;
        [SerializeField] private int m_cardIndex;
        [SerializeField] private int m_atkPoint;
        [SerializeField] private SelectCardEvent m_selectCardEvent;
        [SerializeField] private TextMeshPro m_scoreDisplayer;
        [SerializeField] private BoxCollider2D m_cardCollider;
        [SerializeField] protected bool m_isSelected;
        
        private CardData m_cardData;
        private CardBehavior m_overlappedCard;
        private bool m_canSwawpWithOtherCard;
        private bool m_isDragging;
        private bool m_canClick = true;
        
        private Camera m_mainCamera;
        private Vector3 m_startDraggingPosition;
        private string m_originalName;
        
        // Add these new fields for drag detection
        private Vector3 m_mouseDownPosition;
        private float m_mouseDownTime;
        private bool m_dragIntentDetected = false;
        
        // Drag detection thresholds
        private const float k_DragDistanceThreshold = 0.5f; // Minimum distance to start dragging
        private const float k_DragTimeThreshold = 0.2f;     // Minimum time before drag can start
        
        //Const for card positioning
        private const float k_SelectCardYOffset = -2f;
        private const float k_DeselectOffset = -2.5f;
        private const float k_MoveCardTweenDuration = 0.2f;
        
        public BoxCollider2D CardCollider => m_cardCollider;
        public CardData CardData => m_cardData;
        public int CardIndex => m_cardIndex;
        public bool IsSelected => m_isSelected;
        public int AttackPts => m_atkPoint;
        
        public Func<CardBehavior, CardBehavior> IsOverlappedOnCard;
        public Action<CardBehavior,CardBehavior> SwapCardsAction;
        public Action SelectAction;
        public Action DeselectAction;
        
        public void SetCardIndex(int index) => m_cardIndex = index;
        
        public void SetName() => this.gameObject.name = $"{m_originalName} Index {m_cardIndex}";

        public void ChangeCardState(CardState state)
        {
            m_cardState = state;
        }
        
        public void SetData(CardData cardData)
        {
            m_cardData = cardData;
            m_atkPoint = m_cardData.Info.AttackPoint;
            m_cardVisual.ChangeCardVisual(m_cardData);
        }

        public void SetCanClick(bool canClick)
        {
            m_canClick = canClick;
        }

        public void BringCardToFront(bool toFront)
        {
            if (toFront)
            {
                m_cardVisual.BringCardToFront();
            }
            else
            {
                m_cardVisual.ResetToDefaultLayer();
            }
        }
        #region Unity Cycle Methods
        
        private void Awake()
        {
            m_mainCamera = Camera.main;
            m_originalName = gameObject.name;
        }

        private void Update()
        {
            if (!m_isDragging) return;
            var newPos = GetWorldMousePosition();
            newPos.y -= 0.75f;
            transform.position = newPos;
        }
        
        #endregion
        
        #region Mouse Events Handling
        
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
                    //Make card visual is on top of the other card, regardless of the order of the card in the pile
                    m_cardVisual.BringCardToFront();
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
            
            m_playSFXEvent.Raise(SFX.ClickCard);

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
            m_cardVisual.ResetToDefaultLayer();
        }

        public void ResetSelection(bool shouldReturnEnergy = false)
        {
            m_canClick = true;
            m_isSelected = false;
            var currentLocal = transform.localPosition;
            currentLocal.y = k_DeselectOffset;
            transform.localPosition = currentLocal;
            if (shouldReturnEnergy)
            {
                OnDeselect();
            }
        }
        
        #endregion
        
        #region Score/Atk Pts Display
        
        public void ShowAtkPointHUD()
        {
            m_scoreDisplayer.gameObject.SetActive(true);
            m_scoreDisplayer.text = $"+{m_atkPoint}";
        }

        public void HideAtkPointHUD()
        {
            m_scoreDisplayer.gameObject.SetActive(false);
        }
        
        #endregion
        
        /// <summary>
        /// Set current position as start dragging position.
        /// </summary>
        public void SetHandPosition(Vector3 position)
        {
            m_startDraggingPosition = position;
        }
        
        protected virtual void OnSelect()
        {
            SelectAction?.Invoke();
            m_selectCardEvent.Raise(new SelectCardEventData
            {
                CardIndex = m_cardIndex,
                EnergyCost = m_cardData.Info.EnergyCost,
                IsSelected = true
            });
        }
        
        protected virtual void OnDeselect()
        {
            DeselectAction?.Invoke();
            m_selectCardEvent.Raise(new SelectCardEventData
            {
                CardIndex = m_cardIndex,
                EnergyCost = m_cardData.Info.EnergyCost,
                IsSelected = false
            });
        }

        private Vector3 GetWorldMousePosition()
        {
            var mousePos = Input.mousePosition;
            mousePos = m_mainCamera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            return mousePos;
        }
        
        #region Tweening

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
        
        #endregion  
    }
}
