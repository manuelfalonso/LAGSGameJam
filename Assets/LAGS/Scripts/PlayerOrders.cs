using System.Collections.Generic;
using LAGS.Pub;
using UnityEngine;

namespace LAGS.Player
{
    public class PlayerOrders : MonoBehaviour
    {
        [SerializeField] private Interact _interact;
        private List<Order> _orders = new ();
        
        public void AddOrder(Order order)
        {
            Debug.Log("Order added " + order);
            _orders.Add(order);
        }
    }
}
