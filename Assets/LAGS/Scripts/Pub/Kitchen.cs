using System.Collections.Generic;
using System.Linq;
using LAGS.Player;
using LAGS.Pub;
using UnityEngine;

namespace LAGS
{
    public class Kitchen : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<SpriteRenderer> _visualPlates;
        [SerializeField] private AudioClip _dishesSFX;
        [SerializeField] private AudioClip _scribbleSFX;
        private List<Plate> _visualQueue = new();
        private List<Plate> _plates = new();
        private List<Plate> _readyPlates = new();

        private AudioSource audioSource;
        

        private void AddOrders(Order order)
        {
            foreach (var plate in order.Plates)
            {
                plate.ChangeStatus(OrderStatus.InProgress);
                _plates.Add(plate);
            }
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
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
                
                var visualPlate = _visualPlates.FirstOrDefault(visualPlate => !visualPlate.gameObject.activeInHierarchy);

                if(!visualPlate) { return; }
                
                _readyPlates.Add(plate);
                _plates.RemoveAt(i);
                
                if (!visualPlate)
                {
                    _visualQueue.Add(plate);
                    continue;
                }
                
                visualPlate.gameObject.SetActive(true);
                visualPlate.sprite = plate.PlateSprite;

                audioSource.PlayOneShot(_dishesSFX);
            }
        }

        public void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out PlayerOrders player)) { return; }

            while (player.Orders.Count > 0)
            {
                if (player.Orders.Count == 1)
                    audioSource.PlayOneShot(_scribbleSFX);

                var order = player.Orders[0];
                AddOrders(order);
                player.RemoveOrder(order);
            }

            if (_readyPlates.Count == 0 || player.HasBothHandsOccupied()) return;
            
            for (var i = 0; i < _readyPlates.Count;)
            {
                if (!player.IsHandOccupied(true))
                {
                    player.SetPlates(_readyPlates[i], true);
                    
                    var visual = _visualPlates.Find(plate => plate.sprite == _readyPlates[i].PlateSprite);
                    if (visual != null)
                    {
                        if (_visualQueue.Count > 0)
                        {
                            visual.sprite = _visualQueue[0].PlateSprite;
                            _visualQueue.RemoveAt(0);
                        }
                        else
                        {
                            visual.gameObject.SetActive(false);
                            visual.sprite = null;
                        }
                    }
                    
                    _readyPlates.RemoveAt(i);
                }
                else if (!player.IsHandOccupied(false))
                {
                    player.SetPlates(_readyPlates[i], false);
                    
                    var visual = _visualPlates.Find(plate => plate.sprite == _readyPlates[i].PlateSprite);
                    if (visual != null)
                    {
                        if (_visualQueue.Count > 0)
                        {
                            visual.sprite = _visualQueue[0].PlateSprite;
                            _visualQueue.RemoveAt(0);
                        }
                        else
                        {
                            visual.gameObject.SetActive(false);
                            visual.sprite = null;
                        }
                    }
                    
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
