using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LAGS
{
    public class Cheff : MonoBehaviour
    {
        private static readonly int AboutSneeze = Animator.StringToHash("AboutSneeze");
        private static readonly int Sneeze = Animator.StringToHash("Sneeze");
        
        [SerializeField] private Animator _animator;
        [MinMaxSlider(2,5), SerializeField] private Vector2 _timeToSneeze;
        [SerializeField] private float _previousSneezeTime;
        [SerializeField] private float _sneezeTime;
        private float _currentTimeToSneeze;
        private bool _justSneeze;
        public bool JustSneeze => _justSneeze;
        
        private void Start()
        {
            GetRandomTime();
        }

        private void Update()
        {
            if (_currentTimeToSneeze > 0)
            {
                _currentTimeToSneeze -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(nameof(CallAboutSneeze));
                GetRandomTime();
            }
        }

        private IEnumerator CallAboutSneeze()
        {
            _animator.SetTrigger(AboutSneeze);
            yield return new WaitForSeconds(_previousSneezeTime);
            StartCoroutine(nameof(SneezeDuration));
        }

        private IEnumerator SneezeDuration()
        {
            _justSneeze = true;
            _animator.SetBool(Sneeze, true);
            yield return new WaitForSeconds(_sneezeTime);
            _animator.SetBool(Sneeze, false);
            _justSneeze = false;
        }
        
        private void GetRandomTime()
        {
            _currentTimeToSneeze = Random.Range(_timeToSneeze.x, _timeToSneeze.y);
        }
    }
}
