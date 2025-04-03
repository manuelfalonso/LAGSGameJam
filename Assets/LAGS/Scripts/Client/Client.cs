using System.Collections;
using System.Collections.Generic;
using LAGS.Managers.Pub;
using LAGS.Pub;
using SombraStudios.Shared.AI;
using UnityEngine;
using UnityEngine.AI;

namespace LAGS.Clients
{
    public class Client : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform _head;
        [SerializeField] private GameObject _fov;
        [Header("Basic Settings")]
        [SerializeField] private float _clientSpeed;
        [SerializeField] private float _timeToOrder;
        [Header("Field of View Settings")]
        [SerializeField] private float _fovAngle;
        [SerializeField] private LayerMask _obstacleMask;
        [Header("Alert Settings")] 
        [SerializeField] private float _focusTime = 2f;
        [SerializeField] private int _ratDetectedReducePoints = 60;
        [SerializeField] private int _ratAlertedReducePoints = 10;
        private float _currentFocusTime;
        
        private Plate _plate;
        private Table _table;
        private Chair _chair;
        private bool _isAlert;
        private bool _isEscaping;
        private bool _isSitting;
        private bool _pointsReduced;
        private List<Transform> _rats = new();
        private LineOfSight.IsInSightData _data;

        private void Start()
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.speed = _clientSpeed;
        }

        private void Update()
        {
            RotateClient();
            CheckRatDetection();
            ClientAlert();
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if(!_isSitting) { return; }
            
            if(!other.TryGetComponent<Rat>(out var rat)) { return; }
            
            if (!_rats.Contains(rat.transform))
            {
                _rats.Add(rat.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(!_isSitting) { return; }
            
            if(!other.TryGetComponent<Rat>(out var rat)) { return; }
            
            if (_rats.Contains(rat.transform))
            {
                _rats.Remove(rat.transform);
            }
        }

        private void RotateClient()
        {
            if (Vector3.Distance(transform.position, _agent.destination) <= _agent.stoppingDistance)
            {
                if (_isEscaping)
                {
                    Destroy(gameObject);
                    return;
                }
                
                transform.position = _chair.transform.position;
                transform.localRotation = _chair.SittingPosition.localRotation * Quaternion.Euler(0, 0, 90);
                _agent.isStopped = true;
                _isSitting = true;
                _fov.SetActive(true);
                return;
            }
            
            if (!(_agent.velocity.sqrMagnitude > 0.01f)) { return; }
            
            var direction = _agent.velocity.normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        private void CheckRatDetection()
        {
            if(_rats.Count <= 0 || _isAlert) { return; }

            foreach (var rat in _rats)
            {
                var data = new LineOfSight.IsInSightData
                {
                    StartPoint = _head,
                    EndPoint = rat,
                    ObstaclesMask = _obstacleMask,
                    Is2D = true
                };
                
                if (!LineOfSight.IsInFieldOfViewAndInSight(data, _fovAngle)) { continue; }
                
                _isAlert = true;
                _currentFocusTime = _focusTime;
                _data = data;
                _table.ReducePoints(_ratAlertedReducePoints, Reason.RatAlerted);
                break;
            }
        }

        private void ClientAlert()
        {
            if(!_isAlert || _pointsReduced) { return; }

            if (!LineOfSight.IsInFieldOfViewAndInSight(_data, _fovAngle))
            {
                _isAlert = false;
                return;
            }
            
            var direction = _data.EndPoint.position - _head.transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            _currentFocusTime -= Time.deltaTime;
            if (_currentFocusTime <= 0)
            {
                _pointsReduced = true;
                _table.ReducePoints(_ratDetectedReducePoints, Reason.RatDetected);
            }
        }

        public void Escape()
        {
            _table.LeaveTable(this);
            _isSitting = false;
            _isEscaping = true;
            _agent.isStopped = false;
            _agent.SetDestination(PubManager.Instance.PubDoors.position);
        }
        
        private void GetPlate()
        {
            _plate = PubManager.Instance.GetRandomPlate();
            _table.AddPlate(_plate);
        }
        
        public void AssignTable(Table table)
        {
            _table = table;
            _table.SitClient(this);
            
            _chair = _table.GetFreeChair();
            _chair.AssignClient(this);
            
            _agent.updateRotation = true;
            _agent.SetDestination(_chair.SittingPosition.position);
            
            StartCoroutine(nameof(WaitToOrder));
        }

        private IEnumerator WaitToOrder()
        {
            yield return new WaitForSeconds(_timeToOrder);
            GetPlate();
        }
    }
}
