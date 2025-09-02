using System;
using SGGames.Scripts.Data;
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
        
        //Const for card positioning
        private const float k_SelectCardYOffset = -2f;
        private const float k_DeselectOffset = -2.5f;
        private const float k_MoveCardTweenDuration = 0.2f;
        
        public BoxCollider2D CardCollider => m_cardCollider;
        public CardData CardData => m_cardData;
        public int CardIndex => m_cardIndex;
        public bool IsSelected => m_isSelected;
        public int AttackPts => m_atkPoint;
        
        public Func<CardBehavior, CardBehavior> IsOverlappedOnCard
        {
            set => m_cardInputHandler.IsOverlappedOnCard = value;
        }

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
            m_cardInputHandler.CanClick = canClick;
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
        }

        private void OnDestroy()
        {
            m_cardInputHandler.OnMouseDownOnCard -= HandleMouseDown;
            m_cardInputHandler.OnCardClicked -= HandleClick;
            m_cardInputHandler.OnDragStarted -= HandleDragStart;
            m_cardInputHandler.OnDragEnded -= HandleDragEnd;
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
                OnSelectTween();
            }
            else
            {
                OnDeselectTween();
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
        
        
        #region Tweening

        public void TweenCardToPosition(Vector3 position, Action onFinish)
        {
            m_cardInputHandler.CanClick = false;
            transform.LeanMove(position, k_MoveCardTweenDuration)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(() =>
                {
                    transform.position = position;
                    onFinish?.Invoke();
                    m_cardInputHandler.CanClick = true;
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
        
        private void OnSelectTween()
        {
            m_cardInputHandler.CanClick = false;
            transform.LeanMoveLocalY(k_SelectCardYOffset, k_MoveCardTweenDuration).setEase(LeanTweenType.easeOutCirc).setOnComplete(OnCompleteTween);
        }
        
        private void OnDeselectTween()
        {
            m_cardInputHandler.CanClick = false;
            transform.LeanMoveLocalY(k_DeselectOffset, k_MoveCardTweenDuration).setEase(LeanTweenType.easeOutCirc).setOnComplete(OnCompleteTween);
        }
        
        #endregion  
    }
}
