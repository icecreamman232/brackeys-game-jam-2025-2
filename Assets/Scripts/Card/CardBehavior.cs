using System;
using SGGames.Scripts.Data;
using TMPro;
using UnityEngine;

namespace SGGames.Scripts.Card
{
    public enum CardState
    {
        InPile, InHand, InDiscard,
    }
    public class CardBehavior : MonoBehaviour
    {
        [SerializeField] private CardAnimation m_cardAnimation;
        [SerializeField] private CardInputHandler m_cardInputHandler;
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
        private string m_originalName;
        
        public BoxCollider2D CardCollider => m_cardCollider;
        public CardData CardData => m_cardData;
        public int CardIndex
        {
            get => m_cardIndex;
            set => m_cardIndex = value;
        }

        public bool IsSelected => m_isSelected;
        public int AttackPts => m_atkPoint;
        
        public CardAnimation Animation => m_cardAnimation;
        public CardInputHandler Input => m_cardInputHandler;
        
        public Func<CardBehavior, CardBehavior> IsOverlappedOnCard
        {
            set => m_cardInputHandler.IsOverlappedOnCard = value;
        }

        public Action<CardBehavior,CardBehavior> SwapCardsAction;
        public Action SelectAction;
        public Action DeselectAction;
        
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
            m_originalName = gameObject.name;
            m_cardInputHandler.OnMouseDownOnCard += HandleMouseDown;
            m_cardInputHandler.OnCardClicked += HandleClick;
            m_cardInputHandler.OnDragStarted += HandleDragStart;
            m_cardInputHandler.OnDragEnded += HandleDragEnd;

            m_cardAnimation.OnCompletedSelectTween += OnCompleteTween;
            m_cardAnimation.OnCompletedDeselectTween += OnCompleteTween;
        }

        private void OnDestroy()
        {
            m_cardInputHandler.OnMouseDownOnCard -= HandleMouseDown;
            m_cardInputHandler.OnCardClicked -= HandleClick;
            m_cardInputHandler.OnDragStarted -= HandleDragStart;
            m_cardInputHandler.OnDragEnded -= HandleDragEnd;
            
            m_cardAnimation.OnCompletedSelectTween -= OnCompleteTween;
            m_cardAnimation.OnCompletedDeselectTween -= OnCompleteTween;
        }
        
        #endregion
        
        #region Mouse Events Handling
        
        private void HandleMouseDown()
        {
            m_playSFXEvent.Raise(SFX.ClickCard);
        }

        private void HandleClick()
        {
            // Original click behavior
            if (!m_isSelected)
            {
                m_cardInputHandler.CanClick = false;
                m_cardAnimation.PlaySelectAnimation();
            }
            else
            {
                m_cardInputHandler.CanClick = false;
                m_cardAnimation.PlayDeselectAnimation();
            }
        }
        
        private void HandleDragStart()
        {
            //Bring card visual to front when drag starts
            m_cardVisual.BringCardToFront();
        }
        private void HandleDragEnd()
        {
            // Original drag end behavior
            if (m_cardInputHandler.CanSwap)
            {
                SwapCardsAction?.Invoke(this, m_cardInputHandler.OverlappedCard);
            }
            else
            {
                transform.position = m_cardInputHandler.StartDragPositon;
            }

            m_isSelected = false;
            m_cardVisual.ResetToDefaultLayer();
        }

        public void ResetSelection(bool shouldReturnEnergy = false)
        {
            m_cardInputHandler.CanClick = true;
            m_isSelected = false;
            m_cardAnimation.ResetAnimation();
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
            m_cardInputHandler.StartDragPositon = position;
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
        private void OnCompleteTween()
        {
            m_isSelected = !m_isSelected;
            m_cardInputHandler.CanClick = true;
            if (m_isSelected)
            {
                OnSelect();
            }
            else
            {
                OnDeselect();
            }
        }
    }
}
