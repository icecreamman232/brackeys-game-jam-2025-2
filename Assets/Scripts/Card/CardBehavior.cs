using UnityEngine;

namespace SGGames.Scripts.Card
{
    public class CardBehavior : MonoBehaviour
    {
        [SerializeField] protected bool m_isDragging;
        private Camera m_camera;
        private const float k_OffsetY = 0.75f; 
        
        private void Awake()
        {
            m_camera = Camera.main;
        }

        private void Update()
        {
            if (!m_isDragging) return;
            var pos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            pos.y -= k_OffsetY;
            pos.z = 0;
            transform.position = pos;
        }

        private void OnMouseDrag()
        {
            m_isDragging = true;
            OnDragging();
        }
        
        private void OnMouseUp()
        {
            m_isDragging = false;
            OnReleased();
        }

        protected virtual void OnDragging()
        {
            
        }

        protected virtual void OnReleased()
        {
            
        }
    }
}
