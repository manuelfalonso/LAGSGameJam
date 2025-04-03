using System.Collections.Generic;
using LAGS.Pub;
using UnityEngine;

namespace LAGS.Player
{
    public class PlayerOrders : MonoBehaviour
    {
        [SerializeField] private Interact _interact;
        private List<Order> _orders = new ();
        public List<Order> Orders => _orders;
        
        public Plate LeftPlate;
        public Plate RightPlate;
        
        public void AddOrder(Order order)
        {
            _orders.Add(order);
        }
    }
}
