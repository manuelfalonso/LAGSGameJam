using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LAGS
{
    public class Cheff : MonoBehaviour
    {
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
                Sneeze();
                GetRandomTime();
            }
        }

        private void Sneeze()
        {
            StartCoroutine(nameof(SneezeDuration));
        }

        private IEnumerator SneezeDuration()
        {
            _justSneeze = true;
            yield return new WaitForSeconds(_sneezeTime);
            _justSneeze = false;
        }
        
        private void GetRandomTime()
        {
            _currentTimeToSneeze = Random.Range(_timeToSneeze.x, _timeToSneeze.y);
        }
    }
}
