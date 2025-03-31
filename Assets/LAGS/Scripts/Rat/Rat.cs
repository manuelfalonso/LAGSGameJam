using SombraStudios.Shared.Patterns.Behavioural.Observer.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    public class Rat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VoidEventChannelSO _onRatDeathEvent;

        private void OnDestroy()
        {
            if (_onRatDeathEvent != null)
                _onRatDeathEvent.RaiseEvent();
            else
                Debug.LogWarning("No VoidEventChannelSO assigned to Rat", this);
        }
    }
}
