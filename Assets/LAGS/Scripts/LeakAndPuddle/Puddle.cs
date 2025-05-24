using LAGS.Managers.Pub;
using LAGS.Player;
using SombraStudios.Shared.Patterns.Behavioural.Observer.ScriptableObjects;
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
            if (PubManager.Instance.IsDayOver) { return; }
            
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
                Destroy(transform.parent.gameObject);
            }
        }

        public void Interact(GameObject interactor)
        {
            // Validate if interactor PlayerOrders has the hands empty
            if (interactor.TryGetComponent(out PlayerOrders playerOrders))
            {
                if (playerOrders.HasEmptyHands)
                {
                    // Switch clean animation on Player
                    if (interactor.TryGetComponent(out PlayerAnimations playerAnimations))
                    {
                        playerAnimations.SetClean(true);
                    }

                    _isInteracting = true;
                }
            }
        }

        public void InteractExit(GameObject interactor)
        {
            // Switch clean animation on Player
            if (interactor.TryGetComponent(out PlayerAnimations playerAnimations))
            {
                playerAnimations.SetClean(false);
            }

            // Execute interaction
            _isInteracting = false;
        }
    }
}
