using UnityEngine;

namespace LAGS
{
    public class Interact : MonoBehaviour
    {
        private IInteractable _interactable;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                _interactable = interactable;
                //interactable.Interact();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                _interactable = null;
            }
        }

        // Called from PlayerInput
        public void InteractWith()
        {
            if (_interactable != null)
            {
                _interactable.Interact(gameObject);
            }
        }
    }
}
