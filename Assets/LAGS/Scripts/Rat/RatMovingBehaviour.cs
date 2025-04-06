using SombraStudios.Shared.Utility.Timers.CsharpTimer;
using UnityEngine;
using UnityEngine.AI;

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
        [SerializeField] private string _movementXAxisParameter = "MoveX";
        [SerializeField] private string _movementYAxisParameter = "MoveY";

        private Vector3 _targetPosition;
        private CountdownTimer _countdownTimer;
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _countdownTimer = new CountdownTimer(_movingTime);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_navMeshAgent == null)
                animator.TryGetComponent(out _navMeshAgent);

            if (_rigidbody2D == null)
                animator.TryGetComponent(out _rigidbody2D);

            SamplePosition(animator);

            if (_animator == null)
                _animator = animator;

            _countdownTimer.OnTimerStop -= TransitionToIdle;
            _countdownTimer.OnTimerStop += TransitionToIdle;
            _countdownTimer.Start();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetAnimatorParameters(animator);

            var direction = _targetPosition - animator.transform.position;
            var distance = direction.magnitude;

            if (!_navMeshAgent.hasPath)
                SamplePosition(animator);

            //if (distance <= _distanceToTargetThreshold)
            //{
            //    SamplePosition(animator);
            //}
            //else
            //{
            //    var isInSightData = new SombraStudios.Shared.AI.LineOfSight.IsInSightData
            //    {
            //        StartPoint = animator.transform,
            //        EndPoint = animator.transform,
            //        EndPointOffset = _targetPosition - animator.transform.position,
            //        ObstaclesMask = _obstacleLayerMask
            //    };

            //    if (SombraStudios.Shared.AI.LineOfSight.IsInSight(isInSightData, out var hit))
            //    {
            //        var movement = direction.normalized * _moveSpeed * Time.deltaTime;
            //        animator.transform.position += movement;
            //    }
            //    else
            //    {
            //        SamplePosition(animator);
            //    }
            //}
        }

        private void SetAnimatorParameters(Animator animator)
        {
            animator.SetFloat(_movementXAxisParameter, _rigidbody2D.velocity.x);
            animator.SetFloat(_movementYAxisParameter, _rigidbody2D.velocity.y);
        }

        private void SamplePosition(Animator animator)
        {
            // Sample position on a circle around the rat
            var circlePosition = Random.insideUnitCircle * _rangeRadius;
            _targetPosition = animator.transform.position + (Vector3)circlePosition;

            //Debug.Log("Target position " + _targetPosition);

            while (!NavMesh.SamplePosition(_targetPosition, out var hit, _rangeRadius, 1))
            {
                circlePosition = Random.insideUnitCircle * _rangeRadius;
                _targetPosition = animator.transform.position + (Vector3)circlePosition;

                //Debug.Log("Target position " + _targetPosition);
            }

            _navMeshAgent.SetDestination(_targetPosition);
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
