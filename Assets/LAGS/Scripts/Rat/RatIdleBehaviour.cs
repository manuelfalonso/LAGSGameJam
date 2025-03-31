using SombraStudios.Shared.Utility.Timers.CsharpTimer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    public class RatIdleBehaviour : StateMachineBehaviour
    {
        private const string MOVING_BOOL = "IsMoving";

        [Header("Configuration")]
        [SerializeField] private float _idleTime = 10f;

        private CountdownTimer _countdownTimer;
        private Animator _animator;

        private void Awake()
        {
            _countdownTimer = new CountdownTimer(_idleTime);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_animator == null)
                _animator = animator;

            _countdownTimer.OnTimerStop -= TransitionToMoving;
            _countdownTimer.OnTimerStop += TransitionToMoving;
            _countdownTimer.Start();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _countdownTimer.Tick();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _countdownTimer.Stop();
        }

        private void TransitionToMoving()
        {
            if (_animator != null)
                _animator.SetBool(MOVING_BOOL, true);
            else
                Debug.LogWarning("Animator is null");
        }
    }
}
