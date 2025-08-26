using System;
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
        [SerializeField] private CardState m_cardState = CardState.InPile;
        [SerializeField] private int m_cardIndex;
        [SerializeField] private int m_atkPoint;
        [SerializeField] private SelectCardEvent m_selectCardEvent;
        [SerializeField] private SpriteRenderer m_cardIcon;
        [SerializeField] private TextMeshPro m_atkPointText;
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
        
        public CardState CardState => m_cardState;
        public int CardIndex => m_cardIndex;
        public bool IsSelected => m_isSelected;
        
        public void SetCardIndex(int index) => m_cardIndex = index;
        public int AttackPts => m_atkPoint;
        public BoxCollider2D CardCollider => m_cardCollider;
        
        public Func<CardBehavior, CardBehavior> IsOverlappedOnCard;
        public Action SwapCardsAction;

        private void Awake()
        {
            m_mainCamera = Camera.main;
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

        public void SetAtkPoint(int atkPoint)
        {
            m_atkPoint = atkPoint;
            m_atkPointText.text = atkPoint.ToString();
        }


        private void OnMouseDrag()
        {
            m_isDragging = true;
            var overlappedCard = IsOverlappedOnCard?.Invoke(this);
            if (overlappedCard!=null)
            {
                Debug.Log($"Overlapped on {overlappedCard.gameObject.name}");
                m_canSwawpWithOtherCard = true;
            }
            else
            {
                m_canSwawpWithOtherCard = false;
            }
        }

        private void OnMouseDown()
        {
            if (!m_canClick) return;

            if (!m_isSelected)
            {
                OnSelectTween();
            }
            else
            {
                OnDeselectTween();
            }
        }

        private void OnMouseUp()
        {
            m_isDragging = false;
            
            if (m_canSwawpWithOtherCard)
            {
                
            }
            else
            {
                transform.position = m_startDraggingPosition;
            }

            m_canSwawpWithOtherCard = false;
        }

        public void ResetSelection()
        {
            m_canClick = true;
            m_isSelected = false;
            var currentLocal = transform.localPosition;
            currentLocal.y = k_DeselectOffset;
            transform.localPosition = currentLocal;
        }

        public void SetIcon(Sprite icon)
        {
            m_cardIcon.sprite = icon;
        }
        
        public void ChangeCardState(CardState state)
        {
            m_cardState = state;
        }

        /// <summary>
        /// Set current position as start dragging position.
        /// </summary>
        public void SetHandPosition()
        {
            m_startDraggingPosition = transform.position;
        }
        
        public virtual void OnSelect()
        {
            m_selectCardEvent.Raise(new SelectCardEventData
            {
                CardIndex = m_cardIndex,
                IsSelected = true
            });
        }
        
        public virtual void OnDeselect()
        {
            m_selectCardEvent.Raise(new SelectCardEventData
            {
                CardIndex = m_cardIndex,
                IsSelected = false
            });
        }

        public void ShowAtkPointHUD()
        {
            m_scoreDisplayer.gameObject.SetActive(true);
            m_scoreDisplayer.text = m_atkPoint.ToString();
        }

        public void HideAtkPointHUD()
        {
            m_scoreDisplayer.gameObject.SetActive(false);
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

        private void OnDrawGizmos()
        {
            //if (m_isDragging)
            {
                Gizmos.color = Color.red;
                var cardCenter = m_cardCollider.bounds.center;
                Gizmos.DrawCube(cardCenter, Vector3.one);
            }
        }
    }
}
