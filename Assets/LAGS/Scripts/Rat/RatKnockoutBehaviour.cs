using SombraStudios.Shared.Utility.Timers.CsharpTimer;
using UnityEngine;

namespace LAGS
{
    public class RatKnockoutBehaviour : StateMachineBehaviour
    {
        private const float INITIAL_DESPAWN_CHANCE = 0.2f;

        [Header("Configuration")]
        [SerializeField] private float _knockoutTime = 15f;
        [SerializeField] private float _despawnChanceIncrease = 0.2f;

        private CountdownTimer _countdownTimer;
        private float _currentDespawnChance = INITIAL_DESPAWN_CHANCE;
        private Animator _animator;
        private GameObject _ratGameObject;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_animator == null)
                _animator = animator;

            if (_ratGameObject == null)
                _ratGameObject = animator.gameObject;

            if (_countdownTimer == null)
                _countdownTimer = new CountdownTimer(_knockoutTime);
            else
                _countdownTimer.Reset();

            _countdownTimer.OnTimerStop -= TryToDespawn;
            _countdownTimer.OnTimerStop += TryToDespawn;
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

        private void TryToDespawn()
        {
            var despawnChance = Random.Range(0f, 1f);
            if (despawnChance <= _currentDespawnChance)
            {
                Destroy(_ratGameObject);
            }
            else
            {
                _currentDespawnChance += _despawnChanceIncrease;
                _animator.SetTrigger(RatAnimationParameters.Recover);
            }
        }
    }
}
