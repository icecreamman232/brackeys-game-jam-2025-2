using UnityEngine;

namespace SGGames.Scripts.Card
{
    public class CardBehavior : MonoBehaviour
    {
        [SerializeField] private int m_cardIndex;
        [SerializeField] private SelectCardEvent m_selectCardEvent;
        [SerializeField] protected bool m_isSelected;
        protected bool m_canClick = true;
        
        public bool IsSelected => m_isSelected;
        
        public void SetCardIndex(int index) => m_cardIndex = index;

        private void OnMouseDown()
        {
            if (!m_canClick) return;
            
            if (m_isSelected)
            {
                OnSelectTween();
            }
            else
            {
                OnDeselectTween();
            }
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
            transform.LeanMoveLocalY(-1f, 0.2f).setEase(LeanTweenType.easeOutCirc).setOnComplete(OnCompleteTween);
        }
        
        private void OnDeselectTween()
        {
            m_canClick = false;
            transform.LeanMoveLocalY(-1.5f, 0.2f).setEase(LeanTweenType.easeOutCirc).setOnComplete(OnCompleteTween);
        }
    }
}
