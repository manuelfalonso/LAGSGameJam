using LAGS.Player;
using UnityEngine;

namespace LAGS
{
    public class Interact : MonoBehaviour
    {
        public PlayerOrders PlayerOrders;

        [Header("References")]
        [SerializeField] private GameObject _interactor;

        [Header("Debug")]
        [SerializeField] private bool _showLogs;

        private IDraggable _draggable; 
        private bool _isDragging;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Rats interactions
            if (other.TryGetComponent(out IInteractable interactable))
            {
                if (_showLogs)
                    Debug.Log("Interact enter " + interactable, this);

                interactable.Interact(_interactor);
            }

            //Debug.LogError(_isDragging, this);
            //if (_isDragging) return;

            //// Biombo interactions
            //if (other.TryGetComponent(out IDraggable draggable))
            //{
            //    if (_showLogs)
            //        Debug.Log("Draggable entered " + draggable, this);

            //    _draggable = draggable;
            //}
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            //if (other.TryGetComponent(out IDraggable draggable))
            //{
            //    Debug.LogError(_isDragging, this);
            //    if (_isDragging) return;

            //    if (_showLogs)
            //        Debug.Log("Draggable exited " + draggable, this);

            //    _draggable = null;
            //}

            if (other.TryGetComponent(out IInteractable interactable))
            {
                if (_showLogs)
                    Debug.Log("Interact exit " + interactable, this);

                interactable.InteractExit(_interactor);
            }
        }

        public void Drag()
        {
            if (_isDragging) return;

            // Boxcast 2D to try get component of a draggable to assign it to _draggable
            var hit = Physics2D.BoxCast(
                transform.position, 
                Vector2.one,
                0f, 
                Vector2.zero, 
                0f,
                LayerMask.GetMask("Obstacle"));

            if (hit.collider == null) return;
            if (hit.collider.TryGetComponent(out IDraggable draggable))
            {
                if (_showLogs)
                    Debug.Log("Draggable entered " + draggable, this);

                _draggable = draggable;
            }

            if (_draggable != null)
            {
                if (_showLogs)
                    Debug.Log("Draggable dragged " + _draggable, this);

                _draggable.Drag(_interactor);
                _isDragging = true;
            }
        }

        public void Drop()
        {
            if (!_isDragging) return;

            if (_draggable != null)
            {
                if (_showLogs)
                    Debug.Log("Draggable dropped " + _draggable, this);

                _draggable.Drop(_interactor);
                _isDragging = false;
                _draggable = null;
            }
        }
    }
}
