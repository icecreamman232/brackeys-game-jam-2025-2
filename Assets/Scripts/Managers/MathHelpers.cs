using UnityEngine;

namespace SGGames.Scripts.Core
{
    /// <summary>
    /// Static helper class which contain vary math-type methods
    /// </summary>
    public static class MathHelpers
    {
        public static bool IsContainBound(this Bounds bound, Bounds target)
        {
            return (bound.Contains(target.min) && bound.Contains(target.max));
        }

        /// <summary>
        /// Remaps a value x in interval [A,B], to the proportional value in interval [C,D]
        /// </summary>
        /// <param name="x">The value to remap.</param>
        /// <param name="A">the minimum bound of interval [A,B] that contains the x value</param>
        /// <param name="B">the maximum bound of interval [A,B] that contains the x value</param>
        /// <param name="C">the minimum bound of target interval [C,D]</param>
        /// <param name="D">the maximum bound of target interval [C,D]</param>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            var remappedValue = C + (x-A)/(B-A) * (D - C);
            return remappedValue;
        }
        
        /// <summary>
        /// Returns the angle of this vector, in radians
        /// </summary>
        /// <param name="v">The vector to get the angle of. It does not have to be normalized</param>
        /// <returns></returns>
        public static float GetAngle( this Vector2 v ) => Mathf.Atan2( v.y, v.x );
        public static float DegToRad( this float angDegrees ) => angDegrees * Mathf.Deg2Rad;
        public static float RadToDeg( this float angRadians ) => angRadians * Mathf.Rad2Deg;

        public static bool IsInclusiveRange(this float f, float min, float max)
        {
            return (min <= f) && (f <= max);
        }
        public static bool IsExclusiveRange(this float f, float min, float max)
        {
            return (min < f) && (f < max);
        }

        /// <summary>
        /// Reset position, localScale and rotation value to default value. <br/>
        /// Position => Vector3 (0,0,0) <br/>
        /// LocalScale => Vector3(1,1,1) <br/>
        /// Rotation => Quaternion Identity <br/>
        /// </summary>
        /// <param name="tf"></param>
        public static void Reset(this Transform tf, bool ignoreScale = false)
        {
            tf.position = Vector3.zero;
            if (!ignoreScale)
            {
                tf.localScale = Vector3.one;
            }
            tf.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Convert number to percentage
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ValueToPercent(float value)
        {
            return value * 100;
        }
        
        /// <summary>
        /// Return floating point value from percentage
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static float PercentToValue(float percent)
        {
            return percent / 100;
        }

        /// <summary>
        /// Return percent of value x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static float Percent(float x, float percent)
        {
            return (x / 100f * percent);
        }

        public static Vector2 GetPerpendicularDirection(Vector2 vector)
        {
            var normalized = vector.normalized;
            return new Vector2(-normalized.y, normalized.x);
        }

        public static Vector2 GetPointAtDistanceFromLine(Vector2 point, Vector2 pointA, Vector2 pointB, float distance)
        {
            Vector2 lineDirection = (pointB - pointA).normalized;
            Vector2 offsetDirection = GetPerpendicularDirection(lineDirection); // Perpendicular to line

            return point + offsetDirection * distance;
        }
    }
}
