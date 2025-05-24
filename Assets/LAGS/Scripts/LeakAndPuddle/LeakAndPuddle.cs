using System;
using System.Collections;
using LAGS.Managers.Pub;
using UnityEngine;

namespace LAGS
{
    public class LeakAndPuddle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ParticleSystem _leakParticleSystem;
        [SerializeField] private Puddle _puddle;

        [Header("Settings")]
        [SerializeField] private LeakAndPuddleData _leakAndPuddleData;
        
        private Coroutine _puddleCoroutine;

        private void Start()
        {
            _puddleCoroutine = StartCoroutine(TransitionToPuddle());
        }

        private void OnEnable()
        {
            PubManager.Instance.DayFinished.RemoveListener(DayOver);
            PubManager.Instance.DayFinished.AddListener(DayOver);
        }

        private void OnDisable()
        {
            PubManager.Instance.DayFinished.RemoveListener(DayOver);
        }

        private void DayOver()
        {
            StopCoroutine(_puddleCoroutine);
        }

        private IEnumerator TransitionToPuddle()
        {
            yield return new WaitForSeconds(_leakAndPuddleData.TimeTransitionToPuddle);

            // Stop the leak particle system
            if (_leakParticleSystem != null)
            {
                _leakParticleSystem.Stop();
                _leakParticleSystem.gameObject.SetActive(false);
            }

            // Activate the puddle
            if (_puddle != null)
            {
                _puddle.gameObject.SetActive(true);
                _puddle.Setup(_leakAndPuddleData);
            }
        }
    }
}
