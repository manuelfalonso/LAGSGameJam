using UnityEngine;

namespace LAGS
{
    [CreateAssetMenu(fileName = "PlateAnimationData", menuName = "LAGS/Plate Animation Data")]
    public class PlateAnimationData : ScriptableObject
    {
        public Vector3 LeftLocalPosition;
        public Vector3 RightLocalPosition;
        public int LeftSpriteOrder;
        public int RightSpriteOrder;
    }
}
