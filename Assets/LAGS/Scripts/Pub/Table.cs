using System;
using System.Collections.Generic;
using System.Linq;
using LAGS.Clients;
using UnityEngine;

namespace LAGS.Pub
{
    public class Table : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<Chair> _chairs = new();
        [SerializeField] private int _startingPoints = 100;
        [SerializeField] private float _addedTimeToWait = 15f;
        [SerializeField] private int _reducePointsForPlateDelayed = 5;
        [SerializeField] private int _extraReducePoints = 1;
        private List<Client> _clients = new();
        private bool _isEmpty;
        private List<Plate> _plates = new();
        private Order _order;
        private int _currentPoints;
        private int _currentPointsToReduce;
        private float _currentTimeToWait;
        
        private Dictionary<Reason, bool> _reasons = new()
        {
            {Reason.RatAlerted, false},
            {Reason.RatDetected, false},
            {Reason.ChefDetected, false},
            {Reason.PuddleDetected, false},
            {Reason.PlateDelayed, false}
        };

        public bool IsEmpty => _isEmpty;
        private bool CanTakeOrder() 
        { 
            if(_order == null) { return false; }
            if(_isEmpty) { return false; }

            var canTake = false;

            foreach (var plate in _order.Plates.Where(plate => plate.Status is OrderStatus.Ready))
            {
                canTake = true;
            }

            return canTake;
        }

        private void Start()
        {
            _isEmpty = true;
        }

        private void Update()
        {
            if (_order?.Plates[0].Status is not OrderStatus.InProgress) { return; }
            
            if(_order.AreAllPlatesReady) { return; }

            if (_currentTimeToWait > 0)
            {
                _currentTimeToWait -= Time.deltaTime;
                if (_currentTimeToWait <= 0)
                {
                    _currentPointsToReduce += _reducePointsForPlateDelayed;
                }
            }
            else
            {
                _currentPointsToReduce += _extraReducePoints;
            }
        }

        private void PrepareOrder()
        {
            _order = new Order(_plates, this);
            Debug.Log("Order prepared");
        }
        
        public void SitClient(Client client)
        {
            InitializeTable();
            _clients.Add(client);
            _isEmpty = false;
        }
        
        private void InitializeTable()
        {
            _currentPoints = _startingPoints;
            _currentPointsToReduce = 0;
            foreach (var reason in _reasons.Keys.ToList())
            {
                _reasons[reason] = false;
            }
        }
        
        public void LeaveTable(Client client)
        {
            _clients.Remove(client);
            _isEmpty = true;
            _plates.Clear();
        }
        
        public void AddPlate(Plate plate)
        {
            _plates.Add(plate);

            if (_plates.Count == _clients.Count)
            {
                PrepareOrder();
            }
        }

        public int GetTotalSpaces()
        {
            return _chairs.Count(chair => chair.IsEmpty);
        }

        public void Interact(GameObject interactor)
        {
            if(!CanTakeOrder()) { return; }

            if (!interactor.TryGetComponent(out Interact interact)) { return; }
            
            var player = interact.PlayerOrders;
            
            if (!player.Orders.Contains(_order))
            {
                if (_order.Plates[0].Status is not OrderStatus.Request) { return; }
                
                _order.SetOrderStatus(OrderStatus.InProgress);
                player.AddOrder(_order);

                _currentTimeToWait = _order.Plates.Max(p => p.TimeToPrepare) + _addedTimeToWait;
                return;
            }

            foreach (var plate in _order.Plates)
            {
                if (plate.Equals(player.LeftPlate))
                {
                    Debug.Log("Delivered");
                    player.RemovePlates(true);
                    plate.ChangeStatus(OrderStatus.Delivered);
                }
                else if (plate.Equals(player.RightPlate))
                {
                    player.RemovePlates(false);
                    plate.ChangeStatus(OrderStatus.Delivered);
                }
            }
            
            if (!_order.AreAllPlatesReady) { return; }

            if (_currentPointsToReduce > 0)
            {
                ReducePoints(_currentPointsToReduce, Reason.PlateDelayed);
            }
        }

        public Chair GetFreeChair()
        {
            return _chairs.FirstOrDefault(chair => chair.IsEmpty);
        }

        public void ReducePoints(int amount, Reason reason)
        {
            if(_reasons[reason]) { return; }
            
            _reasons[reason] = true;
            _currentPoints -= amount;
            
            if(_currentPoints > 0) { return; }
            
            foreach (var client in _clients)
            {
                client.Escape();
            }
        }
    }
    
    public enum Reason
    {
        RatAlerted,
        RatDetected,
        ChefDetected,
        PuddleDetected,
        PlateDelayed
    }
}