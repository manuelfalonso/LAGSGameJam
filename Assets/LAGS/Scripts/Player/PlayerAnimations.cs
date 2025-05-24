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

        private bool _isDayOver;

        private void OnEnable()
        {
            PubManager.Instance.DayFinished.RemoveListener(DayOver);
            PubManager.Instance.DayFinished.AddListener(DayOver);
        }

        private void OnDisable()
        {
            PubManager.Instance.DayFinished.RemoveListener(DayOver);
        }

        private void Update()
        {
            if (_isDayOver) { return; }
            
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

        private void DayOver()
        {
            _isDayOver = true;
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
