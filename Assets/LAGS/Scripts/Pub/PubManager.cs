using System.Collections.Generic;
using System.Linq;
using LAGS.Clients;
using LAGS.Pub;
using SombraStudios.Shared.Patterns.Creational.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LAGS.Managers.Pub
{
    public class PubManager : Singleton<PubManager>
    {
        [Header("Pub")]
        [SerializeField] private Transform _pubDoors;
        [SerializeField] private Transform _clientSpawnPoint;
        public Transform PubDoors => _pubDoors;
        
        [Header("Day")]
        [Tooltip("* 60 is the value in minutes"), SerializeField] private float _dayDuration;
        [Tooltip("* 60 is the value in minutes"), SerializeField] private float _closeDoorTime;
        private float _currentTime;
        private bool _isDayOver;
        private bool _isDoorClosed;
        
        [Header("Clients")]
        [SerializeField] private GameObject _clientPrefab;
        [SerializeField] private float _minTimeToNewClient;
        [SerializeField] private float _maxTimeToNewClient;
        private float _currentTimeToNewClient;
        private List<Client> _clients;
        private bool CanGenerateClient => IsThereEmptyTable() && !_isDoorClosed && !_isDayOver;
        
        [Header("Tables")]
        [SerializeField] private List<Table> _tables = new();
        
        [Header("Plates")]
        [SerializeField] private Plate[] _plates;
        

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
            _clients = new List<Client>();
        }
        #endregion

        #region Day

        private void ManageDayTime()
        {
            if(_isDayOver) { return; }

            if (_currentTime >= _dayDuration)
            {
                _isDayOver = true;
            }
            else
            {
                _currentTime += Time.deltaTime;
            }
            
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
            var tables = GetFreeTables();
            var maxClients = tables.Select(table => table.GetTotalSpaces()).Prepend(0).Max();
            var totalClients = Random.Range(1, maxClients + 1);
            var table = GetRandomTable(totalClients);
            
            if(table is null) { return; }
            
            for (var i = 0; i < totalClients; i++)
            {
                var client = Instantiate(_clientPrefab, _pubDoors.position, Quaternion.identity, _clientSpawnPoint)
                    .GetComponent<Client>();
                _clients.Add(client);
                client.AssignTable(table);
            }
        }
        
        private void RemoveClient(Client client)
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

        public List<Table> GetFreeTables()
        {
            return _tables.Where(table => table.IsEmpty).ToList();
        }

        public Table GetRandomTable(int maxClients)
        {
            var tables = _tables
                .Where(table => table.IsEmpty)
                .Where(table => table.GetTotalSpaces() >= maxClients)
                .ToList();
            
            return tables.Count == 0 ? null : tables[Random.Range(0, tables.Count)];
        }
        #endregion

        #region Plates

        public Plate GetRandomPlate()
        {
            return _plates[Random.Range(0, _plates.Length)];
        }

        #endregion
    }
}