using SombraStudios.Shared.Utility.Timers.CsharpTimer;
using UnityEngine;

namespace LAGS
{
    public class RatMovingBehaviour : StateMachineBehaviour
    {
        private const string MOVING_BOOL = "IsMoving";

        [Header("Configuration")]
        [SerializeField] private float _movingTime = 10f;
        [SerializeField] private float _rangeRadius = 5f;
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _distanceToTargetThreshold = 0.1f;
        [SerializeField] private LayerMask _obstacleLayerMask;

        private Vector3 _targetPosition;
        private CountdownTimer _countdownTimer;
        private Animator _animator;

        private void Awake()
        {
            _countdownTimer = new CountdownTimer(_movingTime);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SamplePosition(animator);

            if (_animator == null)
                _animator = animator;

            _countdownTimer.OnTimerStop -= TransitionToIdle;
            _countdownTimer.OnTimerStop += TransitionToIdle;
            _countdownTimer.Start();
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var direction = _targetPosition - animator.transform.position;
            var distance = direction.magnitude;

            if (distance <= _distanceToTargetThreshold)
            {
                SamplePosition(animator);
            }
            else
            {
                var isInSightData = new SombraStudios.Shared.AI.LineOfSight.IsInSightData
                {
                    StartPoint = animator.transform,
                    EndPoint = animator.transform,
                    EndPointOffset = _targetPosition - animator.transform.position,
                    ObstaclesMask = _obstacleLayerMask
                };

                if (SombraStudios.Shared.AI.LineOfSight.IsInSight(isInSightData, out var hit))
                {
                    var movement = direction.normalized * _moveSpeed * Time.deltaTime;
                    animator.transform.position += movement;
                }
                else
                {
                    SamplePosition(animator);
                }

                //var movement = direction.normalized * _moveSpeed * Time.deltaTime;
                //animator.transform.position += movement;
            }
        }

        private void SamplePosition(Animator animator)
        {
            // Sample position on a circle around the rat
            var circlePosition = Random.insideUnitCircle * _rangeRadius;
            _targetPosition = animator.transform.position + (Vector3)circlePosition;
        }

        private void TransitionToIdle()
        {
            if (_animator != null)
                _animator.SetBool(MOVING_BOOL, false);
            else
                Debug.LogWarning("Animator is null");
        }
    }
}
