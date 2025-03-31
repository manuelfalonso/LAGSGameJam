using SombraStudios.Shared.Patterns.Behavioural.Observer.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Transform> _spanPoints = new();
        [SerializeField] private VoidEventChannelSO _onRatDeathEvent;

        [Header("Settings")]
        [SerializeField] private SpawnData _spawnData;

        [Header("Debug")]
        [SerializeField] private bool _spawnAtStart = true;

        private Transform _previousSpawnPoint;
        private int _currentSpawns;

        private void OnEnable()
        {
            _onRatDeathEvent.OnEventRaised -= DecreaseSapwn;
            _onRatDeathEvent.OnEventRaised += DecreaseSapwn;
        }

        private void OnDisable()
        {
            _onRatDeathEvent.OnEventRaised -= DecreaseSapwn;
        }

        private void Start()
        {
            if (_spawnAtStart)
            {
                InvokeRepeating(nameof(Spawn), 0f, _spawnData.TimeBetweenSpawns);
            }
        }

        private void DecreaseSapwn()
        {
            _currentSpawns--;
        }

        private void Spawn()
        {
            if (_currentSpawns < _spawnData.MaxSpawns)
            {
                Instantiate(_spawnData.ObjectToSpawn, GetRandomSpawnPoint().position, Quaternion.identity);
                _currentSpawns++;
            }
        }

        private Transform GetRandomSpawnPoint()
        {
            if (_spawnData.AllowConsecutiveRepeatSpawns)
            {
                return _spanPoints[Random.Range(0, _spanPoints.Count)];
            }
            else
            {
                Transform spawnPoint = _spanPoints[Random.Range(0, _spanPoints.Count)];
                while (spawnPoint == _previousSpawnPoint)
                {
                    spawnPoint = _spanPoints[Random.Range(0, _spanPoints.Count)];
                }
                _previousSpawnPoint = spawnPoint;
                return spawnPoint;
            }
        }
    }
}
