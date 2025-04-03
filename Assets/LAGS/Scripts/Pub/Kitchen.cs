using System.Collections;
using System.Collections.Generic;
using LAGS.Pub;
using UnityEngine;

namespace LAGS
{
    public class Kitchen : MonoBehaviour, IInteractable
    {
        private List<Order> _orders = new();

        public void AddOrders(Order order)
        {
            _orders.Add(order);
            for (int i = 0; i < order.Plates.Count; i++)
            {
                var plate = order.Plates[i];
                plate.Status = OrderStatus.InProgress;
                order.Plates[i] = plate;
            }
        }
        
        private void Update()
        {
            if(_orders.Count <= 0) { return; }
            
            foreach (var order in _orders)
            {
                order.Tick();
            }
        }

        public void Interact(GameObject interactor)
        {
            
        }
    }
}
