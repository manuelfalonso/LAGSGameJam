using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    [CreateAssetMenu(fileName = "SpawnData", menuName = "LAGS/Spawn Data")]
    public class SpawnData : ScriptableObject
    {
        [Header("Spawn Settings")]
        public GameObject ObjectToSpawn;
        public float TimeBetweenSpawns = 10f;
        public bool AllowConsecutiveRepeatSpawns = false;
        public int MaxSpawns = 3;
    }
}
