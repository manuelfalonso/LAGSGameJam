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
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _clientSpeed;
        [SerializeField] private float _timeToOrder;
        [SerializeField] private Transform _head;
        [SerializeField] private float _fovAngle;
        [SerializeField] private LayerMask _obstacleMask;
        private bool _isSitting;
        private Plate _plate;
        private Table _table;
        private Chair _chair;
        private List<Transform> _rats = new();

        private void Start()
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.speed = _clientSpeed;
        }

        private void Update()
        {
            if(!_isSitting)
            {
                RotateClient();
            }
            
            if(_rats.Count <= 0) { return; }

            foreach (var rat in _rats)
            {
                var data = new LineOfSight.IsInSightData
                {
                    StartPoint = _head,
                    EndPoint = rat,
                    ObstaclesMask = _obstacleMask,
                    Is2D = true
                };
                var isInLine = LineOfSight.IsInFieldOfViewAndInSight(data, _fovAngle);
            }
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
                transform.position = _chair.transform.position;
                transform.localRotation = _chair.SittingPosition.localRotation * Quaternion.Euler(0, 0, 90);
                _agent.isStopped = true;
                _isSitting = true;
                return;
            }
            
            if (!(_agent.velocity.sqrMagnitude > 0.01f)) { return; }
            
            var direction = _agent.velocity.normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
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
