using SombraStudios.Shared.Structs;
using UnityEngine;

namespace LAGS
{
    [CreateAssetMenu(fileName = "SpawnData", menuName = "LAGS/Spawn Data")]
    public class SpawnData : ScriptableObject
    {
        [Header("Spawn Settings")]
        public GameObject[] ObjectToSpawn;
        public RangedFloat TimeBetweenSpawns;
        public bool AllowConsecutiveRepeatSpawns = false;
        public int MaxSpawns = 3;
        public bool SpawnAtStart = true;
    }
}
