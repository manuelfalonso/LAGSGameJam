using SombraStudios.Shared.Extensions;
using SombraStudios.Shared.VFX.CameraShake;
using UnityEngine;

namespace LAGS
{
    public class Kickable : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private CameraShakeController _cameraShakeController;
        [SerializeField] private CameraShakeDataSO _cameraShakeData;

        [Header("Configuration")]
        [SerializeField] private float _kickForce = 10f;
        [SerializeField] private ForceMode2D _forceMode = ForceMode2D.Impulse;

        [Header("Debug")]
        [SerializeField] private bool _showLogs;

        private void Awake()
        {
            this.SafeInit(ref _rigidbody2D);
            this.SafeInit(ref _cameraShakeController);
        }

        public void Interact(GameObject interactor)
        {
            if (_showLogs)
                Debug.Log("Kickable interacted with", this);

            Kick(interactor);
        }

        private void Kick(GameObject kicker)
        {
            // Move the kickable in the opposite direction of the kicker using phsyics
            Vector2 direction = (transform.position - kicker.transform.position).normalized;
            _rigidbody2D.AddForce(direction * _kickForce, _forceMode);

            // Shake the camera
            _cameraShakeController.ShakeCamera(_cameraShakeData);
        }
    }
}
