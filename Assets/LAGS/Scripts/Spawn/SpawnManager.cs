using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private List<ActiveSpawn> _activeSpawns = new();

        [Header("Debug")]
        [SerializeField] private bool _showLogs;

        private void Start()
        {
            StartSpawning();
        }

        private void StartSpawning()
        {
            foreach (var spawn in _activeSpawns)
            {
                if (spawn.SpawnData.SpawnAtStart)
                {
                    StartCoroutine(SpawnCoroutine(spawn.SpawnData));
                }
            }
        }

        private IEnumerator SpawnCoroutine(SpawnData spawnData)
        {
            while (true)
            {
                Spawn(spawnData);
                yield return new WaitForSeconds(spawnData.TimeBetweenSpawns.RandomValue);
            }
        }

        public void DecreaseSapwn(SpawnData spawnToDecrease)
        {
            var spawn = _activeSpawns.Find(x => x.SpawnData == spawnToDecrease);

            if (spawn == null)
            {
                Debug.LogWarning("Spawn not found in active spawns list.", this);
                return;
            }
            
            spawn.CurrentSpawns--;
        }

        public void Spawn(SpawnData spawnToIncrease)
        {
            var spawn = _activeSpawns.Find(x => x.SpawnData == spawnToIncrease);

            if (spawn == null)
            {
                spawn = new ActiveSpawn
                {
                    SpawnData = spawnToIncrease,
                    CurrentSpawns = 0
                };
                _activeSpawns.Add(spawn);
            }

            if (spawn.CurrentSpawns < spawn.SpawnData.MaxSpawns)
            {
                Instantiate(spawn.SpawnData.ObjectToSpawn, GetRandomSpawnPoint(spawn).position, Quaternion.identity);
                spawn.CurrentSpawns++;
            }
        }

        private Transform GetRandomSpawnPoint(ActiveSpawn spawn)
        {
            if (spawn.SpawnData.AllowConsecutiveRepeatSpawns)
            {
                return spawn.SpawnPoints[Random.Range(0, spawn.SpawnPoints.Count)];
            }
            else
            {
                Transform spawnPoint = spawn.SpawnPoints[Random.Range(0, spawn.SpawnPoints.Count)];                
                while (spawnPoint == spawn.PreviousSpawnPoint)
                {
                    spawnPoint = spawn.SpawnPoints[Random.Range(0, spawn.SpawnPoints.Count)];
                }
                spawn.PreviousSpawnPoint = spawnPoint;
                return spawnPoint;
            }
        }

        [System.Serializable]
        public class ActiveSpawn
        {
            [Header("Spawn Settings")]
            public SpawnData SpawnData;
            public List<Transform> SpawnPoints = new();
            [Header("Debug")]
            public Transform PreviousSpawnPoint;
            public int CurrentSpawns;
        }
    }
}
