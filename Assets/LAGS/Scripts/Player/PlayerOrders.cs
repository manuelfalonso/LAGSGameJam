using System.Collections.Generic;
using LAGS.Pub;
using UnityEngine;
using UnityEngine.Events;

namespace LAGS.Player
{
    public class PlayerOrders : MonoBehaviour
    {
        [SerializeField] private Interact _interact;
        [SerializeField] private SpriteRenderer _leftPlate;
        [SerializeField] private SpriteRenderer _rightPlate;
        
        private List<Order> _orders = new ();
        public List<Order> Orders => _orders;
        public bool HasEmptyHands => !LeftPlate.HasValue && !RightPlate.HasValue;
        
        [HideInInspector] public Plate? LeftPlate;
        [HideInInspector] public Plate? RightPlate;
        
        public UnityEvent<bool> OnePlateSet;
        public UnityEvent<bool> TwoPlateSet;
        public UnityEvent<Order> NewOrder;
        public UnityEvent<Order> OrderInKitchen;
        
        public void AddOrder(Order order)
        {
            NewOrder?.Invoke(order);
            _orders.Add(order);
        }
        
        public void RemoveOrder(Order order)
        {
            _orders.Remove(order);
            OrderInKitchen?.Invoke(order);
        }

        public void SetPlates(Plate plate, bool leftPlate)
        {
            if (leftPlate)
            {
                LeftPlate = plate;
                OnePlateSet?.Invoke(true);
                _leftPlate.sprite = plate.PlateSprite;
                _leftPlate.gameObject.SetActive(true);
            }
            else
            {
                RightPlate = plate;
                TwoPlateSet?.Invoke(true);
                _rightPlate.sprite = plate.PlateSprite;
                _rightPlate.gameObject.SetActive(true);
            }
        }

        public void RemovePlates(bool leftPlate)
        {
            if (leftPlate)
            {
                LeftPlate = null;
                OnePlateSet?.Invoke(false);
                _leftPlate.gameObject.SetActive(false);
                _leftPlate.sprite = null;
            }
            else
            {
                RightPlate = null;
                TwoPlateSet?.Invoke(false);
                _rightPlate.gameObject.SetActive(false);
                _rightPlate.sprite = null;
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
