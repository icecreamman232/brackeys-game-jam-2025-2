using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SGGames.Scripts.Core
{
    /// <summary>
    /// Static helper class which contains method for vary components, scripts and situations.
    /// </summary>
    public static class GameHelper
    {
        #region UI
        public static void SetAlpha(this Image image, float alpha)
        {
            var curColor = image.color;
            curColor.a = alpha;
            image.color = curColor;
        }
        
        public static Color Alpha1(this Color color)
        {
            color.a = 1;
            return color;
        }

        public static Color Alpha0(this Color color)
        {
            color.a = 0;
            return color;
        }

        public static CanvasGroup Activate(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            return canvasGroup;
        }

        public static CanvasGroup Deactivate(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            return canvasGroup;
        }
        
        #endregion
        
        #region Randomness
        public static string GetUniqueID()
        {
            return Guid.NewGuid().ToString();
        }

        public static void Shuffle<T>(List<T> list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, count);
                //Swap via deconstruction
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        
        #endregion
        
        #region Vector

        public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vector.x, y ?? vector.y);
        }

        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }

        public static Vector2 Add(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(vector.x + (x ?? 0), vector.y + (y ?? 0));
        }
        
        public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
        }
        
        #endregion
        
        #region Coroutine

        public static IEnumerable<WaitForEndOfFrame> WaitForFrame(int frameNumber)
        {
            for (int i = 0; i < frameNumber; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        
        #endregion
        
        #region Enum - Flag

        /// <summary>
        /// Check whether flag value is in group flag values. Works like LayerMask Unity
        /// </summary>
        /// <param name="input"></param>
        /// <param name="group"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsInEnumGroup<T>(T input, T group) where T : Enum
        {
            int inputValue = Convert.ToInt32(input);
            int groupValue = Convert.ToInt32(group);
            return (inputValue & groupValue) != groupValue;
        }
        #endregion
    }
}
