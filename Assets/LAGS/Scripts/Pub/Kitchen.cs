using System.Collections.Generic;
using LAGS.Pub;
using UnityEngine;

namespace LAGS
{
    public class Kitchen : MonoBehaviour, IInteractable
    {
        private List<Plate> _plates = new();
        private List<Plate> _readyPlates = new();

        public void AddOrders(Order order)
        {
            foreach (var plate in order.Plates)
            {
                plate.ChangeStatus(OrderStatus.InProgress);
                _plates.Add(plate);
            }
        }
        
        private void Update()
        {
            TickPlates();
        }

        private void TickPlates()
        {
            if (_plates.Count == 0) return;

            for (var i = _plates.Count - 1; i >= 0; i--)
            {
                var plate = _plates[i];

                if (plate.Status != OrderStatus.Ready)
                {
                    _plates[i] = plate.Tick();
                }
                
                if (plate.Status != OrderStatus.Ready) { continue; }
                
                Debug.Log("Plate is ready");;
                _readyPlates.Add(plate);
                _plates.RemoveAt(i);
            }
        }

        public void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out Interact interact)) { return; }

            var player = interact.PlayerOrders;

            while (player.Orders.Count > 0)
            {
                var order = player.Orders[0];
                Debug.Log("Adding order to kitchen");
                AddOrders(order);
                player.RemoveOrder(order);
            }

            if (_readyPlates.Count == 0 || player.HasBothHandsOccupied()) return;
            
            for (var i = 0; i < _readyPlates.Count;)
            {
                if (!player.IsHandOccupied(true))
                {
                    player.SetPlates(_readyPlates[i], true);
                    _readyPlates.RemoveAt(i);
                }
                else if (!player.IsHandOccupied(false))
                {
                    player.SetPlates(_readyPlates[i], false);
                    _readyPlates.RemoveAt(i);
                }
                else { break; }

                if (player.HasBothHandsOccupied()) break;
            }
        }

        public void InteractExit(GameObject interactor)
        {
            // noop
        }
    }
}
