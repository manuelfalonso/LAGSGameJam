using System.Collections.Generic;
using System.Linq;
using LAGS.Pub;
using SombraStudios.Shared.Patterns.Creational.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LAGS.Managers.Pub
{
    public class PubManager : Singleton<PubManager>
    {
        [Header("Day")]
        [Tooltip("* 60 is the value in minutes"), SerializeField] private float _dayDuration;
        [Tooltip("* 60 is the value in minutes"), SerializeField] private float _closeDoorTime;
        private float _currentTime;
        private bool _isDayOver;
        private bool _isDoorClosed;
        
        [Header("Clients")]
        [SerializeField] private GameObject _clientPrefab;
        [SerializeField] private Transform _clientSpawnPoint;
        [SerializeField] private float _minTimeToNewClient;
        [SerializeField] private float _maxTimeToNewClient;
        private float _currentTimeToNewClient;
        private List<GameObject> _clients;
        private bool CanGenerateClient => IsThereEmptyTable() && (!_isDoorClosed && !_isDayOver);
        
        [Header("Tables")]
        [SerializeField] private List<Table> _tables = new();

        #region Monobehaviour

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if(_isDayOver) { return; }
            
            ManageDayTime();
            NewClientTimer();
        }

        #endregion
        
        #region Other

        private void Initialize()
        {
            _isDayOver = false;
            _isDoorClosed = false;
            _currentTime = 0;
            _currentTimeToNewClient = Random.Range(_minTimeToNewClient, _maxTimeToNewClient);
            _clients = new List<GameObject>();
        }
        #endregion

        #region Day

        private void ManageDayTime()
        {
            if(_isDayOver) { return; }
            
            if (_currentTime >= _dayDuration) { _isDayOver = true; }
            else { _currentTime += Time.deltaTime; }
            
            if(_isDoorClosed) { return; }
            
            if (_currentTime >= _closeDoorTime)
            {
                _isDoorClosed = true;
            }
        }

        #endregion
        
        #region Clients

        private void NewClientTimer()
        {
            if(!CanGenerateClient) { return; }
            
            if (_currentTimeToNewClient <= 0)
            {
                _currentTimeToNewClient = Random.Range(_minTimeToNewClient, _maxTimeToNewClient);
                GenerateNewClient();
            }
            else
            {
                _currentTimeToNewClient -= Time.deltaTime;
            }
        }

        private void GenerateNewClient()
        {
            var client = Instantiate(_clientPrefab, _clientSpawnPoint.position, _clientSpawnPoint.rotation);
            _clients.Add(client);
        }
        
        private void RemoveClient(GameObject client)
        {
            _clients.Remove(client);
            Destroy(client);
        }

        #endregion
        
        #region Tables
        public bool IsThereEmptyTable()
        {
            return _tables.Any(table => table.IsEmpty);
        }

        public Table GetRandomTable()
        {
            var emptyTables = _tables.Where(table => table.IsEmpty).ToList();
            
            return emptyTables[Random.Range(0, emptyTables.Count)];
        }
        #endregion
    }
}