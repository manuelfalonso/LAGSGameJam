using System;
using UnityEngine;

namespace LAGS
{
    public class Draggable : MonoBehaviour, IDraggable
    {
        [Header("Properties")]
        [SerializeField] private float _followPower = 10f;

        [Header("Debug")]
        [SerializeField] private bool _showLogs;

        //private bool _isDragging;

        private Transform _followTarget;

        private void Update()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            if (_followTarget == null) return;

            // Follw the target with a lerp
            transform.position = Vector3.Lerp(transform.position, _followTarget.position, Time.deltaTime * _followPower);
        }

        public void Drag(GameObject interactor)
        {
            //if (_showLogs)
            //    Debug.Log("Kickable interacted with", this);

            if (interactor.TryGetComponent(out PlayerBehaviour player))
            {
                _followTarget = player.DragFollowTarget;
                //transform.SetParent(player.DragFollowTarget, true);

                // Disable all the colliders attached to this gameobject
                foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
                {
                    collider.enabled = false;
                }

                foreach (Collider collider in GetComponentsInChildren<Collider>())
                {
                    collider.enabled = false;
                }
            }
        }

        public void Drop(GameObject interactor)
        {
            //transform.SetParent(null);
            _followTarget = null;

            // Enable all the colliders attached to this gameobject
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = true;
            }

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
        }
    }
}
