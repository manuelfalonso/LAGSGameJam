using System;
using System.Collections;
using System.Collections.Generic;
using LAGS.Managers.Pub;
using UnityEngine;

namespace LAGS
{
    public class PlayerAnimations : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _animator;

        [Header("Properties")]
        [SerializeField] private string _moveXParameterName = "MoveX";
        [SerializeField] private string _moveYParameterName = "MoveY";
        [SerializeField] private string _isMovingParameterName = "IsMoving";
        [SerializeField] private string _onePlateParameterName = "OnePlate";
        [SerializeField] private string _twoPlateParameterName = "TwoPlates";
        [SerializeField] private string _cleanParameterName = "IsCleaning";

        // Update is called once per frame
        void Update()
        {
            if (PubManager.Instance.IsDayOver) { return; }
            
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            var moveX = Input.GetAxisRaw("Horizontal");
            var moveY = Input.GetAxisRaw("Vertical");

            if (_animator == null)
            {
                Debug.LogError("Missing reference", this);
                return;
            }

            if (moveX != 0 || moveY != 0)
            {
                _animator.SetBool(_isMovingParameterName, true);
            }
            else
            {
                _animator.SetBool(_isMovingParameterName, false);
            }

            _animator.SetFloat(_moveXParameterName, moveX);
            _animator.SetFloat(_moveYParameterName, moveY);
        }

        public void SetOnePlate(bool value)
        {
            if (_animator == null)
            {
                Debug.LogError("Missing reference", this);
                return;
            }

            _animator.SetBool(_onePlateParameterName, value);
        }

        public void SetTwoPlates(bool value)
        {
            if (_animator == null)
            {
                Debug.LogError("Missing reference", this);
                return;
            }

            _animator.SetBool(_twoPlateParameterName, value);
        }

        public void SetClean(bool value)
        {
            if (_animator == null)
            {
                Debug.LogError("Missing reference", this);
                return;
            }

            _animator.SetBool(_cleanParameterName, value);
        }
    }
}
