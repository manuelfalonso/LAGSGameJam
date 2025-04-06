using System.Collections.Generic;
using System.Linq;
using LAGS.Player;
using LAGS.Pub;
using UnityEngine;
using UnityEngine.UI;

namespace LAGS
{
    public class Notepad : MonoBehaviour
    {
        [SerializeField] private List<Image> _image;
        private PlayerOrders _playerOrders;
        
        private void Awake()
        {
            _playerOrders = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerOrders>();
        }

        private void Start()
        {
            _playerOrders.NewOrder.AddListener(OnNewOrder);
            _playerOrders.OrderInKitchen.AddListener(OnOrderInKitchen);
        }

        private void OnDestroy()
        {
            _playerOrders.NewOrder.RemoveListener(OnNewOrder);
            _playerOrders.OrderInKitchen.RemoveListener(OnOrderInKitchen);
        }

        private void OnOrderInKitchen(Order order)
        {
            foreach (var spriteRenderer in _image)
            {
                spriteRenderer.enabled = false;
                spriteRenderer.sprite = null;
            }
        }

        private void OnNewOrder(Order order)
        {
            if (_image.All(image => image.enabled)) { return; }

            var visual = _image.Where(visualPlate => !visualPlate.enabled).ToList();
            var plates = order.Plates;
            
            if (plates.Count <= visual.Count)
            {
                CompleteInfoOverPlates(plates, visual);
            }
            else
            {
                CompleteInfoOverVisuals(plates, visual);
            }
        }

        private void CompleteInfoOverPlates(List<Plate> plate, List<Image> renderers)
        {
            for (var i = 0; i < plate.Count; i++)
            {
                renderers[i].sprite = plate[i].PlateSprite;
                renderers[i].enabled = true;
            }
        }

        private void CompleteInfoOverVisuals(List<Plate> plate, List<Image> renderers)
        {
            for (var i = 0; i < renderers.Count -1; i++)
            {
                renderers[i].sprite = plate[i].PlateSprite;
                renderers[i].enabled = true;
            }
        }
    }
}
