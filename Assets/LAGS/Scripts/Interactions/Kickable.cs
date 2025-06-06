﻿using SombraStudios.Shared.Extensions;
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
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioClip onHitSFX;

        [Header("Configuration")]
        [SerializeField] private float _kickForce = 10f;
        [SerializeField] private ForceMode2D _forceMode = ForceMode2D.Impulse;

        [Header("Debug")]
        [SerializeField] private bool _showLogs;

        private AudioSource audioSource;

        private void Awake()
        {
            this.SafeInit(ref _rigidbody2D);
            this.SafeInit(ref _cameraShakeController);
            this.SafeInit(ref _animator);
            audioSource = GetComponent<AudioSource>();
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

            // Set the Animation Parameter
            if (_animator != null)
            {
                _animator.SetTrigger(RatAnimationParameters.IsKnockout);
            }
            else
            {
                Debug.LogWarning("No Animator assigned to Kickable", this);
            }

            //sfx
            audioSource.PlayOneShot(onHitSFX);
        }

        public void InteractExit(GameObject interactor)
        {
            // noop
        }
    }
}
