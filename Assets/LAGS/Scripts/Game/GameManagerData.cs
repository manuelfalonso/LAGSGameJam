using UnityEngine;

namespace LAGS
{
    [CreateAssetMenu(fileName = "GameManagerData", menuName = "LAGS/GameManagerData")]
    public class GameManagerData : ScriptableObject
    {
        [Header("Day Settings")]
        public float DayDurationInSeconds = 180f;
    }
}
