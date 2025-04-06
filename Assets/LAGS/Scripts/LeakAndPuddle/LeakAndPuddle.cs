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

        private void Start()
        {
            Invoke(nameof(TransitionToPuddle), _leakAndPuddleData.TimeTransitionToPuddle);
        }

        private void TransitionToPuddle()
        {
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
