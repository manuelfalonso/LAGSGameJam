using UnityEngine;

namespace LAGS
{
    [CreateAssetMenu(fileName = "LeakAndPuddleData", menuName = "LAGS/LeakAndPuddleData")]
    public class LeakAndPuddleData : ScriptableObject
    {
        public float TimeTransitionToPuddle = 5f;
        public float TimeToClearPuddle = 6f;
        [Tooltip("Extra time to clear the puddle before it dissapears.This is to avoid the visuals of the puddle to go to tiny.")]
        public float TImeToClearPuddleMargin = 1f;
    }
}
