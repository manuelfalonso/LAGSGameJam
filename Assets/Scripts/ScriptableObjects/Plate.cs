using UnityEngine;

namespace LAGS.Pub
{
    [CreateAssetMenu(fileName = "Plate", menuName = "LAGS/Plate")]
    public class Plate : ScriptableObject
    {
        public Sprite PlateSprite;
        public float TimeToPrepare;
    }
}
