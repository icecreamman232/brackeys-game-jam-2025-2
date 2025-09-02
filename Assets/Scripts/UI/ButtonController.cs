using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SGGames.Scripts.UI
{
    public class ButtonController : Selectable
    {
        [SerializeField] private PlaySFXEvent m_playSFXEvent;
        [SerializeField] private Sprite m_normalSprite;
        [SerializeField] private Sprite m_onClickSprite;
        [SerializeField] private Color m_normalColor;
        [SerializeField] private Color m_hoverColor;
        
        public Action OnClickAction;

        public override void OnPointerDown(PointerEventData eventData)
        {
            ((Image)targetGraphic).sprite = m_onClickSprite;
            m_playSFXEvent.Raise(SFX.ButtonClick);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            ((Image)targetGraphic).sprite = m_normalSprite;
            base.OnPointerUp(eventData);
            OnClickButton();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            targetGraphic.color = m_hoverColor;
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            targetGraphic.color = m_normalColor;
            base.OnPointerExit(eventData);
        }
        
        protected virtual void OnClickButton()
        {
            OnClickAction?.Invoke();
        }
    }
}

