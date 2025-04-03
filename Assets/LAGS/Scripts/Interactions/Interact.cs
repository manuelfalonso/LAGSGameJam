using LAGS.Player;
using UnityEngine;

namespace LAGS
{
    public class Interact : MonoBehaviour
    {
        public PlayerOrders PlayerOrders;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(gameObject);
            }
        }
    }
}
