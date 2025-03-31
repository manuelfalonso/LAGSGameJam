using System;
using System.Collections.Generic;
using System.Linq;
using LAGS.Clients;
using LAGS.Player;
using UnityEngine;

namespace LAGS.Pub
{
    public class Table : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<Chair> _chairs = new();
        private List<Client> _clients = new();
        private bool _isEmpty;
        private List<Plate> _plates = new();
        private Order _order;

        public bool IsEmpty => _isEmpty;
        private bool CanTakeOrder => _order is { Status: OrderStatus.Request } && !_isEmpty;

        private void Start()
        {
            _isEmpty = true;
        }

        private void PrepareOrder()
        {
            Debug.Log("Prepare Order");
            _order = new Order(_plates);
        }
        
        public void SitClient(Client client)
        {
            _clients.Add(client);
            _isEmpty = false;
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
            if(!CanTakeOrder) { return; }

            if (interactor.TryGetComponent(out PlayerOrders player))
            {
                switch (_order.Status)
                {
                    case OrderStatus.Request:
                        _order.Status = OrderStatus.InProgress;
                        player.AddOrder(_order);
                        break;
                    case OrderStatus.Ready:
                        _order.Status = OrderStatus.Delivered;
                        break;
                }
            }
        }

        public Chair GetFreeChair()
        {
            return _chairs.FirstOrDefault(chair => chair.IsEmpty);
        }
    }
}