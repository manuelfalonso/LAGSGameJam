using System.Collections.Generic;
using LAGS.Pub;
using UnityEngine;
using UnityEngine.Events;

namespace LAGS.Player
{
    public class PlayerOrders : MonoBehaviour
    {
        [SerializeField] private Interact _interact;
        private List<Order> _orders = new ();
        public List<Order> Orders => _orders;
        
        public Plate? LeftPlate;
        public Plate? RightPlate;
        
        public UnityEvent<bool> OnePlateSet;
        public UnityEvent<bool> TwoPlateSet;
        
        public void AddOrder(Order order)
        {
            Debug.Log("Adding order");
            _orders.Add(order);
        }
        
        public void RemoveOrder(Order order)
        {
            _orders.Remove(order);
        }

        public void SetPlates(Plate plate, bool leftPlate)
        {
            if (leftPlate)
            {
                LeftPlate = plate;
                OnePlateSet?.Invoke(true);
            }
            else
            {
                RightPlate = plate;
                TwoPlateSet?.Invoke(true);
            }
        }

        public void RemovePlates(bool leftPlate)
        {
            if (leftPlate)
            {
                LeftPlate = null;
                OnePlateSet?.Invoke(false);
            }
            else
            {
                RightPlate = null;
                TwoPlateSet?.Invoke(false);
            }
        }
        
        public void ClearPlates()
        {
            LeftPlate = null;
            RightPlate = null;
        }
        
        public bool HasBothHandsOccupied()
        {
            return LeftPlate is not null || RightPlate is not null;
        }
        
        public bool IsHandOccupied(bool leftPlate)
        {
            if (leftPlate)
            {
                return LeftPlate is not null;
            }
            else
            {
                return RightPlate is not null;
            }
        }
    }
}
