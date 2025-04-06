using LAGS.Managers.Inputs;
using UnityEngine;

namespace LAGS
{
    [SelectionBase]
    public class PlayerBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Interact _interact;
        [SerializeField] private Transform _dragFollowTarget;

        [Header("Properties")]
        [SerializeField] private string _dragActionName;

        public Transform DragFollowTarget => _dragFollowTarget;

        private ActionContexts _draggableContext;

        #region Unity Messages
        private void OnEnable()
        {
            UnsubscribeFromInputs();
            SubscribeToInputs();
        }

        private void OnDisable()
        {
            UnsubscribeFromInputs();
        }
        #endregion


        #region Private Methods
        private void SubscribeToInputs()
        {
            _draggableContext = new ActionContexts
            {
                ActionStarted = DragAction,
                ActionCanceled = DropAction,
            };
            InputManager.Instance.SubscribeToAction(_dragActionName, _draggableContext);
        }

        private void DragAction(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (_interact == null)
            {
                Debug.LogError("Missing reference", this);
                return;
            }

            _interact.Drag();
        }

        private void DropAction(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (_interact == null)
            {
                Debug.LogError("Missing reference", this);
                return;
            }

            _interact.Drop();
        }

        private void UnsubscribeFromInputs()
        {
            InputManager.Instance.UnsubscribeFromAction(_dragActionName, _draggableContext);
        }
        #endregion
    }
}
