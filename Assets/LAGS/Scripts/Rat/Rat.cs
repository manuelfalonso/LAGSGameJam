using SombraStudios.Shared.Extensions;
using SombraStudios.Shared.Patterns.Behavioural.Observer.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;

namespace LAGS
{
    public class Rat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;

        [Header("Events")]
        [SerializeField] private VoidEventChannelSO _onRatDeathEvent;

        private void Awake()
        {
            this.SafeInit(ref _agent);
        }

        private void Start()
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        private void OnDestroy()
        {
            if (_onRatDeathEvent != null)
                _onRatDeathEvent.RaiseEvent();
            else
                Debug.LogWarning("No VoidEventChannelSO assigned to Rat", this);
        }
    }
}
