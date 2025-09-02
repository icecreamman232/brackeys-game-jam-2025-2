using System;
using UnityEngine;

namespace SGGames.Scripts.Card
{
    public class CardAnimation : MonoBehaviour
    {
        private Vector3 m_originalPosition;
        
        //Const for card positioning
        private const float k_SelectCardYOffset = 0.5f;
        private const float k_MoveCardTweenDuration = 0.2f;
        
        public event Action OnCompletedSelectTween;
        public event Action OnCompletedDeselectTween;

        public void SetOriginalPosition()
        {
            m_originalPosition = transform.localPosition;
        }
        
        public void PlaySelectAnimation()
        {
            SelectTween();
        }
        
        public void PlayDeselectAnimation()
        {
            DeselectTween();
        }

        public void ResetAnimation()
        {
            var currentLocal = transform.localPosition;
            currentLocal.y = m_originalPosition.y;
            transform.localPosition = currentLocal;
        }
        
        public void TweenCardToPosition(Vector3 position, Action onFinish)
        {
            transform.LeanMove(position, k_MoveCardTweenDuration)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(() =>
                {
                    transform.position = position;
                    onFinish?.Invoke();
                });
        }
        
        private void OnCompleteSelectTween()
        {
            OnCompletedSelectTween?.Invoke();
        }
        
        private void OnCompleteDeselectTween()
        {
            OnCompletedDeselectTween?.Invoke();
        }
        
        private void SelectTween()
        {
            transform.LeanMoveLocalY(m_originalPosition.y + k_SelectCardYOffset, k_MoveCardTweenDuration)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(OnCompleteSelectTween);
        }
        
        private void DeselectTween()
        {
            transform.LeanMoveLocalY(m_originalPosition.y, k_MoveCardTweenDuration)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(OnCompleteDeselectTween);
        }
        
    }
}
