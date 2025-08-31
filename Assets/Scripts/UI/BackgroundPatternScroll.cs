using System;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundPatternScroll : MonoBehaviour
{
   [SerializeField] private Image m_background1;
   [SerializeField] private Image m_background2;
   [SerializeField] private float m_scrollSpeed = 100f; // Units per second
   [SerializeField] private float m_resetDistance = 1500f; // Distance from screen edge before reset
   [SerializeField] private float m_minAngle = 80f; // Minimum angle in degrees (90 = straight up)
   [SerializeField] private float m_maxAngle = 100f; // Maximum angle in degrees
   [SerializeField] private bool m_randomizeOnStart = true; // Randomize direction when starting
   [SerializeField] private bool m_randomizeOnReset = false; // Randomize direction when image resets
   
   private bool m_canRun;
   private RectTransform m_bg1RectTransform;
   private RectTransform m_bg2RectTransform;
   private Vector2 m_imageSize;
   private Vector2 m_scrollDirection;
   private RectTransform m_canvasRectTransform;
   private Vector2 m_screenBounds;
   
   private void Start()
   {
      // Get RectTransforms for position manipulation
      m_bg1RectTransform = m_background1.GetComponent<RectTransform>();
      m_bg2RectTransform = m_background2.GetComponent<RectTransform>();
      
      // Get canvas bounds for reset calculations
      Canvas canvas = GetComponentInParent<Canvas>();
      if (canvas != null)
      {
         m_canvasRectTransform = canvas.GetComponent<RectTransform>();
         m_screenBounds = new Vector2(m_canvasRectTransform.rect.width, m_canvasRectTransform.rect.height);
      }
      else
      {
         m_screenBounds = new Vector2(Screen.width, Screen.height);
      }
      
      // Get the size of one background image
      m_imageSize = new Vector2(m_bg1RectTransform.rect.width, m_bg1RectTransform.rect.height);
      
      // Set initial scroll direction
      if (m_randomizeOnStart)
      {
         SetRandomScrollDirection();
      }
      else
      {
         SetScrollDirection(90f); // Default straight up
      }
      
      // Position backgrounds based on scroll direction
      PositionBackgroundsInitially();
   }
   
   public void Run()
   {
      m_canRun = true;
   }
   
   public void SetRandomScrollDirection()
   {
      float randomAngle = UnityEngine.Random.Range(m_minAngle, m_maxAngle);
      SetScrollDirection(randomAngle);
   }
   
   public void SetScrollDirection(float angleInDegrees)
   {
      // Convert angle to radians
      float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
      
      // Calculate direction vector (Unity's coordinate system: 90 degrees = straight up)
      m_scrollDirection = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
      
      // Reposition backgrounds when direction changes
      if (m_bg1RectTransform != null && m_bg2RectTransform != null)
      {
         PositionBackgroundsInitially();
      }
   }
   
   private void PositionBackgroundsInitially()
   {
      // Calculate the distance between backgrounds based on the movement direction
      // Use the largest dimension to ensure proper coverage
      float separationDistance = Mathf.Max(m_imageSize.x, m_imageSize.y);
      
      // Position second background behind the first in the opposite direction of movement
      Vector2 offset = -m_scrollDirection * separationDistance;
      Vector3 bg2Position = m_bg1RectTransform.anchoredPosition;
      bg2Position.x += offset.x;
      bg2Position.y += offset.y;
      m_bg2RectTransform.anchoredPosition = bg2Position;
   }
   
   private void Update()
   {
      if (!m_canRun) return;
      
      // Move both backgrounds
      MoveBackground(m_bg1RectTransform);
      MoveBackground(m_bg2RectTransform);
   }
   
   private void MoveBackground(RectTransform backgroundTransform)
   {
      // Move the background in the scroll direction
      Vector3 currentPosition = backgroundTransform.anchoredPosition;
      Vector2 movement = m_scrollDirection * m_scrollSpeed * Time.deltaTime;
      currentPosition.x += movement.x;
      currentPosition.y += movement.y;
      
      // Check if the background has moved beyond the screen bounds plus reset distance
      if (IsBackgroundOutOfBounds(currentPosition))
      {
         // Reset position to create infinite scroll effect
         // Place it behind the other background
         RectTransform otherBackground = (backgroundTransform == m_bg1RectTransform) ? m_bg2RectTransform : m_bg1RectTransform;
         
         // Calculate the distance between backgrounds based on the movement direction
         float separationDistance = Mathf.Max(m_imageSize.x, m_imageSize.y);
         Vector2 offset = -m_scrollDirection * separationDistance;
         
         currentPosition.x = otherBackground.anchoredPosition.x + offset.x;
         currentPosition.y = otherBackground.anchoredPosition.y + offset.y;
         
         // Optionally randomize direction when resetting
         if (m_randomizeOnReset)
         {
            SetRandomScrollDirection();
         }
      }
      
      backgroundTransform.anchoredPosition = currentPosition;
   }
   
   private bool IsBackgroundOutOfBounds(Vector3 position)
   {
      // Calculate screen bounds with reset distance buffer
      float halfScreenWidth = m_screenBounds.x * 0.5f;
      float halfScreenHeight = m_screenBounds.y * 0.5f;
      
      // Check if background is beyond screen bounds in the direction of movement
      bool outOfBoundsX = false;
      bool outOfBoundsY = false;
      
      // Check X bounds based on movement direction
      if (m_scrollDirection.x > 0) // Moving right
      {
         outOfBoundsX = position.x > halfScreenWidth + m_resetDistance;
      }
      else if (m_scrollDirection.x < 0) // Moving left
      {
         outOfBoundsX = position.x < -halfScreenWidth - m_resetDistance;
      }
      
      // Check Y bounds based on movement direction
      if (m_scrollDirection.y > 0) // Moving up
      {
         outOfBoundsY = position.y > halfScreenHeight + m_resetDistance;
      }
      else if (m_scrollDirection.y < 0) // Moving down
      {
         outOfBoundsY = position.y < -halfScreenHeight - m_resetDistance;
      }
      
      // Return true if out of bounds in the primary movement direction
      return outOfBoundsX || outOfBoundsY;
   }
   
   // Public method to change angle range at runtime
   public void SetAngleRange(float minAngle, float maxAngle)
   {
      m_minAngle = minAngle;
      m_maxAngle = maxAngle;
   }
   
   // Public method to get current direction angle
   public float GetCurrentAngle()
   {
      return Mathf.Atan2(m_scrollDirection.x, m_scrollDirection.y) * Mathf.Rad2Deg;
   }
}
