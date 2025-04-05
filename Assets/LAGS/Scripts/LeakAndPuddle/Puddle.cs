using SombraStudios.Shared.Patterns.Behavioural.Observer.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    public class Puddle : MonoBehaviour, ICleanable, IInteractable
    {
        [Header("References")]
        [SerializeField] private VoidEventChannelSO _puddleCleaned;

        private float _timeToClearPuddle;
        private float _currentTimeToClearPuddle;
        private float _timeToClearPuddleExtraMargin;
        private bool _isInteracting;


        private void Update()
        {
            if (_isInteracting)
            {
                Clean(Time.deltaTime);
            }
        }

        public void Setup(LeakAndPuddleData data)
        {
            _timeToClearPuddle = data.TimeToClearPuddle;
            _currentTimeToClearPuddle = data.TimeToClearPuddle;
            _timeToClearPuddleExtraMargin = data.TImeToClearPuddleMargin;
        }

        public void Clean(float cleanTime)
        {
            _currentTimeToClearPuddle -= cleanTime;

            // If LocalSize of 1 is timeToClearPuddle, then 0 is cleared
            // Decrease its local size to 0 over time
            transform.localScale = new Vector3(1, 1, 1) * 
                ((_currentTimeToClearPuddle + _timeToClearPuddleExtraMargin) / (_timeToClearPuddle + _timeToClearPuddleExtraMargin));

            // If the puddle is cleared, destroy it
            if ((_currentTimeToClearPuddle - _timeToClearPuddleExtraMargin) <= 0)
            {
                // Destroy the LeakAndPuddle component
                Destroy(transform.parent);
            }
        }

        public void Interact(GameObject interactor)
        {
            // Switch clean animation on Player

            _isInteracting = true;
        }

        public void InteractExit(GameObject interactor)
        {
            // Switch clean animation on Player

            // Execute interaction
            //Clean(Time.deltaTime);
            _isInteracting = false;
        }
    }
}
