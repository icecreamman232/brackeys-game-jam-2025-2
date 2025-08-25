using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SGGames.Scripts.UI
{
    public class ButtonController : Selectable, IPointerClickHandler
    {
        public Action OnClickAction;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickButton();
        }

        protected virtual void OnClickButton()
        {
            OnClickAction?.Invoke();
        }
    }
}

