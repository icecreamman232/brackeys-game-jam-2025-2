using UnityEngine;

namespace SGGames.Scripts.Core
{
    /// <summary>
    /// Static class contains helper method and mask and layer define values
    /// </summary>
    public static class LayerManager
    {
        #region Layers
        public static int PlayerLayer = 6;
        public static int EnemyLayer = 7;
        public static int ObstacleLayer = 8;
        public static int DoorLayer = 9;
        #endregion

        #region Layer Masks
        public static int PlayerMask = 1 << PlayerLayer;
        public static int EnemyMask = 1 << EnemyLayer;
        public static int ObstacleMask = 1 << ObstacleLayer;
        public static int DoorMask = 1 << DoorLayer;
        public static int NoWalkableMask = ObstacleMask | DoorMask | EnemyMask;
        #endregion
        
        /// <summary>
        /// Checking whether layer is in layer mask
        /// </summary>
        /// <param name="layerWantToCheck"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static bool IsInLayerMask(int layerWantToCheck, LayerMask layerMask)
        {
            if (((1 << layerWantToCheck) & layerMask) != 0)
            {
                return true;
            }
            return false;
        }
    }
}
