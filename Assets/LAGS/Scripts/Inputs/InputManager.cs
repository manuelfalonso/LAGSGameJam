using SombraStudios.Shared.Patterns.Creational.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LAGS.Managers.Inputs
{
    public class InputManager : Singleton<InputManager>
    {
        #region Private Variables
        [Header("References")]
        [SerializeField] private InputActionAsset _inputs;

        [Header("Debug")]
        [SerializeField] private bool _showLogs = true;

        private Dictionary<string, InputActionMap> _actionMaps = new();
        private Dictionary<string, InputAction> _inputActions = new();
        private HashSet<string> _previousEnableActions = new();
        #endregion

        #region MonoBehaviour Callbacks
        protected override void Awake()
        {
            base.Awake();

            if (_inputs == null)
            {
                Debug.LogError("InputManager: InputActionAsset is not assigned.");
                this.enabled = false;
                return;
            }
            
            _inputActions ??= new Dictionary<string, InputAction>();
            _actionMaps ??= new Dictionary<string, InputActionMap>();

            foreach (var map in _inputs.actionMaps)
            {
                _actionMaps.Add(map.name, map);
                foreach (var action in map.actions)
                {
                    _inputActions.Add(action.name, action);
                }
            }
            
            EnableAllInputs();
        }

        private void OnEnable()
        {
            EnableAllInputs();
        }
        
        private void OnDisable()
        {
            DisableAllInputs();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Disables all inputs in the InputActionAsset.
        /// </summary>
        public void DisableAllInputs()
        {
            _inputs?.Disable();
        }
        
        /// <summary>
        /// Enables all inputs in the InputActionAsset.
        /// </summary>
        public void EnableAllInputs()
        {
            _inputs?.Enable();
        }

        /// <summary>
        /// Returns the float value of the action.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        /// <returns> The float value of the action. </returns>
        public float GetFloatValue(string actionName)
        {
            if (_inputActions.TryGetValue(actionName, out var action))
            {
                return action.ReadValue<float>();
            }

            ReportActionError(actionName);
            return 0f;
        }

        /// <summary>
        /// Checks if the action is pressed.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        /// <returns> True if the action is pressed, false otherwise. </returns>
        public bool IsActionPressed(string actionName)
        {
            if (_inputActions.TryGetValue(actionName, out var action))
            {
                return action.IsPressed();
            }
            
            ReportActionError(actionName);
            return false;
        }
        
        /// <summary>
        /// Checks if the action is triggered.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        /// <returns> True if the action is triggered, false otherwise. </returns>
        public bool IsActionTriggered(string actionName)
        {
            if (_inputActions.TryGetValue(actionName, out var action))
            {
                return action.triggered;
            }
            
            ReportActionError(actionName);
            return false;
        }
        
        /// <summary>
        /// Gets the Vector2 value of the action.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        /// <returns> The Vector2 value of the action. </returns>
        public Vector2 GetVector2Value(string actionName)
        {
            if (_inputActions.TryGetValue(actionName, out var action))
            {
                return action.ReadValue<Vector2>();
            }
            
            ReportActionError(actionName);
            return Vector2.zero;
        }
        
        /// <summary>
        /// Checks if the action is enabled.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        /// <returns> True if the action is enabled, false otherwise. </returns>
        public bool IsActionEnabled(string actionName)
        {
            if (_inputActions.TryGetValue(actionName, out var action))
            {
                return action.enabled;
            }
            
            ReportActionError(actionName);
            return false;
        }
        
        /// <summary>
        /// Checks if the action map is enabled.
        /// </summary>
        /// <param name="actionMapName"> The name of the action map. </param>
        /// <returns> True if the action map is enabled, false otherwise. </returns>
        public bool IsActionMapEnabled(string actionMapName)
        {
            if (_actionMaps.TryGetValue(actionMapName, out var actionMap))
            {
                return actionMap.enabled;
            }
            
            ReportActionError(actionMapName);
            return false;
        }

        /// <summary>
        /// Enables the action map if it is disabled.
        /// </summary>
        /// <param name="actionName"> The name of the action map. </param>
        public void EnableActionMapIfDisabled(string actionName)
        {
            if (_actionMaps.TryGetValue(actionName, out var actionMap))
            {
                if (!actionMap.enabled)
                {
                    actionMap.Enable();
                }
            }
            else
            {
                ReportActionError(actionName);
            }
        }
        
        /// <summary>
        /// Enables the action if it is disabled.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        public void EnableActionIfDisabled(string actionName)
        {
            if (_inputActions.TryGetValue(actionName, out var action))
            {
                if (!action.enabled)
                {
                    action.Enable();
                }
            }
            else
            {
                ReportActionError(actionName);
            }
        }

        /// <summary>
        /// Disables the action map if it is enabled.
        /// </summary>
        /// <param name="actionName"> The name of the action map. </param>
        public void DisableActionMapIfEnabled(string actionName)
        {
            if (_actionMaps.TryGetValue(actionName, out var actionMap))
            {
                if (actionMap.enabled)
                {
                    actionMap.Disable();
                }
            }
            else
            {
                ReportActionError(actionName);
            }
        }

        /// <summary>
        /// Disables the action if it is enabled.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        public void DisableActionIfEnabled(string actionName)
        {
            if (_inputActions.TryGetValue(actionName, out var action))
            {
                if (action.enabled)
                {
                    action.Disable();
                }
            }
            else
            {
                ReportActionError(actionName);
            }
        }
        
        /// <summary>
        /// Disables the input action.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        public void DisableInputAction(string actionName)
        {
            if(_inputActions.TryGetValue(actionName, out var action))
            {
                action.Disable();
            }
            else
            {
                ReportActionError(actionName);
            }
        }
        
        /// <summary>
        /// Enables the input action.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        public void EnableInputAction(string actionName)
        {
            if(_inputActions.TryGetValue(actionName, out var action))
            {
                action.Enable();
            }
            else
            {
                ReportActionError(actionName);
            }
        }
        
        /// <summary>
        /// Disables the action map.
        /// </summary>
        /// <param name="actionMapName"> The name of the action map. </param>
        public void DisableActionMap(string actionMapName)
        {
            if(_actionMaps.TryGetValue(actionMapName, out var actionMap))
            {
                actionMap.Disable();
            }
            else
            {
                ReportActionError(actionMapName);
            }
        }
        
        /// <summary>
        /// Enables the action map.
        /// </summary>
        /// <param name="actionMapName"> The name of the action map. </param>
        public void EnableActionMap(string actionMapName)
        {
            if(_actionMaps.TryGetValue(actionMapName, out var actionMap))
            {
                actionMap.Enable();
            }
            else
            {
                ReportActionError(actionMapName);
            }
        }

        /// <summary>
        /// Subscribes to the action.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        /// <param name="actionContexts"> The contexts that you want to subscribe to. </param>
        public void SubscribeToAction(string actionName, ActionContexts actionContexts)
        {
            if (!_inputActions.TryGetValue(actionName, out var action))
            {
                ReportActionError(actionName);
                return;
            }
            
            if(actionContexts.ActionStarted != null) { action.started += actionContexts.ActionStarted; }
            if(actionContexts.ActionPerformed != null) { action.performed += actionContexts.ActionPerformed; }
            if(actionContexts.ActionCanceled != null) { action.canceled += actionContexts.ActionCanceled; }
        }
        
        /// <summary>
        /// Unsubscribes from the action.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        /// <param name="actionContexts"> The contexts that you want to unsubscribe from. </param>
        public void UnsubscribeFromAction(string actionName, ActionContexts actionContexts)
        {
            if (!_inputActions.TryGetValue(actionName, out var action))
            {
                ReportActionError(actionName);
                return;
            }
            
            if(actionContexts.ActionStarted != null) { action.started -= actionContexts.ActionStarted; }
            if(actionContexts.ActionPerformed != null) { action.performed -= actionContexts.ActionPerformed; }
            if(actionContexts.ActionCanceled != null) { action.canceled -= actionContexts.ActionCanceled; }
        }

        /// <summary>
        /// Stops all inputs, except the ones in the exceptions list.
        /// </summary>
        /// <param name="exceptions"> The names of the actions that you want to keep enabled. </param>
        public void StopInputs(string[] exceptions = null)
        {
            _previousEnableActions.Clear();

            foreach (var action in 
                     _inputActions.Where(action => action.Value.enabled))
            {
                _previousEnableActions.Add(action.Key);
                action.Value.Disable();
            }

            if(exceptions == null) { return; }
            
            foreach (var actionName in exceptions)
            {
                EnableInputAction(actionName);
            }
        }
        
        /// <summary>
        /// Stops all Input Maps, except the ones in the exceptions list.
        /// </summary>
        /// <param name="exceptions"> The names of the action maps that you want to keep enabled. </param>
        public void StopMapInputs(string[] exceptions = null)
        {
            _previousEnableActions.Clear();

            foreach (var actionMap in 
                     _actionMaps.Where(actionMap => actionMap.Value.enabled))
            {
                _previousEnableActions.Add(actionMap.Key);
                actionMap.Value.Disable();
            }

            if(exceptions == null) { return; }
            
            foreach (var actionMapName in exceptions)
            {
                EnableActionMap(actionMapName);
            }
        }
        
        /// <summary>
        /// Resumes all inputs that were stopped.
        /// </summary>
        public void ResumeInputs()
        {
            foreach (var action in _previousEnableActions)
            {
                EnableInputAction(action);
            }
        }
        
        /// <summary>
        /// Resumes all Input Maps that were stopped.
        /// </summary>
        public void ResumeMapInputs()
        {
            foreach (var actionMap in _previousEnableActions)
            {
                EnableActionMap(actionMap);
            }
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Reports an error if the action is not found.
        /// </summary>
        /// <param name="actionName"> The name of the action. </param>
        private void ReportActionError(string actionName)
        {
            if(!_showLogs) { return; }
            
            Debug.LogError($"InputManager: Action {actionName} not found.");
        }
        #endregion
    }

    public struct ActionContexts
    {
        public ActionContexts(Action<InputAction.CallbackContext> actionStarted,
            Action<InputAction.CallbackContext> actionPerformed, Action<InputAction.CallbackContext> actionCanceled)
        {
            ActionStarted = actionStarted;
            ActionPerformed = actionPerformed;
            ActionCanceled = actionCanceled;
        }
        
        public Action<InputAction.CallbackContext> ActionStarted;
        public Action<InputAction.CallbackContext> ActionPerformed;
        public Action<InputAction.CallbackContext> ActionCanceled;
    }
}