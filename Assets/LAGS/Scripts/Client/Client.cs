using System.Collections;
using System.Collections.Generic;
using LAGS.Managers.Pub;
using LAGS.Pub;
using UnityEngine;
using UnityEngine.AI;
using Transform = log4net.Util.Transform;

namespace LAGS.Clients
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Client : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _clientSpeed;
        [SerializeField] private float _timeToOrder;
        private Plate _plate;
        private Table _table;
        private Chair _chair;
        private bool _isRatNear;
        private List<Transform> _rats;

        private void Start()
        {
            _agent.speed = _clientSpeed;
        }

        private void Update()
        {
            if(!_isRatNear) { return; }

            foreach (var rat in _rats)
            {
                
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if rat
            
            _isRatNear = true;

            // if (!_rats.Contains(rat))
            // {
            //     _rats.Add(rat);
            // }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            //Check if rat
            
            _isRatNear = false;
            
            // if (_rats.Contains(rat))
            // {
            //     _rats.Remove(rat);
            // }
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
