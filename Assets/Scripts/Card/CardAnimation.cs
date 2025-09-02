using System;
using UnityEngine;

namespace SGGames.Scripts.Card
{
    public class CardAnimation : MonoBehaviour
    {
        //Const for card positioning
        private const float k_SelectCardYOffset = -2f;
        private const float k_DeselectOffset = -2.5f;
        private const float k_MoveCardTweenDuration = 0.2f;
        
        private const float k_MovingToPositionTime = 0.7f;
        private const float k_MovingToPositionDelay = 0.05f;

        public event Action OnCompletedSelectTween;
        public event Action OnCompletedDeselectTween;
        
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
            currentLocal.y = k_DeselectOffset;
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
            transform.LeanMoveLocalY(k_SelectCardYOffset, k_MoveCardTweenDuration)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(OnCompleteSelectTween);
        }
        
        private void DeselectTween()
        {
            transform.LeanMoveLocalY(k_DeselectOffset, k_MoveCardTweenDuration)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(OnCompleteDeselectTween);
        }
        
    }
}
