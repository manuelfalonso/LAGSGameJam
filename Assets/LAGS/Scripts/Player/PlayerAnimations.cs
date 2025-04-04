using System;
using System.Collections;
using System.Collections.Generic;
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

        // Update is called once per frame
        void Update()
        {
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
    }
}
