using LAGS.Managers.Pub;
using SombraStudios.Shared.Utility.Timers.CsharpTimer;
using UnityEngine;
using UnityEngine.AI;

namespace LAGS
{
    public class RatMovingBehaviour : StateMachineBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float _movingTime = 10f;
        [SerializeField] private float _rangeRadius = 5f;

        private Vector3 _targetPosition;
        private CountdownTimer _countdownTimer;
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _lastPosition;
        private float _clampedDeltaX;
        private float _clampedDeltaY;

        private void Awake()
        {
            _countdownTimer = new CountdownTimer(_movingTime);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (PubManager.Instance.IsDayOver) { return; }
            
            _lastPosition = animator.transform.position;

            if (_navMeshAgent == null)
                animator.TryGetComponent(out _navMeshAgent);

            if (_rigidbody2D == null)
                animator.TryGetComponent(out _rigidbody2D);

            if (_animator == null)
                _animator = animator;
            
            SamplePosition(animator);

            _countdownTimer.OnTimerStop -= TransitionToIdle;
            _countdownTimer.OnTimerStop += TransitionToIdle;
            _countdownTimer.Start();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (PubManager.Instance.IsDayOver) { return; }
            
            SetAnimatorParameters(animator);

            //Debug.Log($"{_navMeshAgent.hasPath} {_navMeshAgent.pathStatus} {_navMeshAgent.remainingDistance}");
            if (!_navMeshAgent.hasPath || _navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete || _navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)
                SamplePosition(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            _navMeshAgent.ResetPath();
            _animator.SetBool(RatAnimationParameters.IsMoving, false);
        }

        private void SetAnimatorParameters(Animator animator)
        {
            Vector2 currentPosition = animator.transform.position;
            Vector2 deltaPosition = currentPosition - _lastPosition;

            _clampedDeltaX = Mathf.Clamp(deltaPosition.x, -1f, 1f);
            _clampedDeltaY = Mathf.Clamp(deltaPosition.y, -1f, 1f);

            _lastPosition = currentPosition;

            //animator.SetFloat(RatAnimationParameters.MoveX, _rigidbody2D.velocity.x);
            //animator.SetFloat(RatAnimationParameters.MoveY, _rigidbody2D.velocity.y);
            animator.SetFloat(RatAnimationParameters.MoveX, _clampedDeltaX);
            animator.SetFloat(RatAnimationParameters.MoveY, _clampedDeltaY);
        }

        private void SamplePosition(Animator animator)
        {
            ////Debug.Log("1");

            //// Sample position on a circle around the rat
            //var circlePosition = Random.insideUnitCircle * _rangeRadius;
            //_targetPosition = animator.transform.position + (Vector3)circlePosition;

            ////Debug.Log("Target position " + _targetPosition);

            //while (!NavMesh.SamplePosition(_targetPosition, out var hit, _rangeRadius, 1))
            //{
            //    circlePosition = Random.insideUnitCircle * _rangeRadius;
            //    _targetPosition = animator.transform.position + (Vector3)circlePosition;

            //    //Debug.Log("Target position " + _targetPosition);
            //}

            ////Debug.Log("Target successfully sampled " + _targetPosition);

            //_navMeshAgent.SetDestination(_targetPosition);

            _targetPosition = GetReachableRandomPosition();
            if (_targetPosition != Vector3.zero)
            {
                _navMeshAgent.SetDestination(_targetPosition);
            }
        }

        private Vector3 GetReachableRandomPosition()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * _rangeRadius;
                Vector3 randomPos = _animator.transform.position + (Vector3)randomCircle;

                //Debug.Log("Target position " + randomPos);

                if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    if (_navMeshAgent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        return hit.position;
                    }
                }
            }

            Debug.LogWarning("Could not find a reachable position.");
            return Vector3.zero;
        }

        private void TransitionToIdle()
        {
            if (_animator != null)
                _animator.SetBool(RatAnimationParameters.IsMoving, false);
            else
                Debug.LogWarning("Animator is null", this);
        }
    }
}
