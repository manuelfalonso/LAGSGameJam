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
        [SerializeField] private GameObject _sneezePreview;
        [MinMaxSlider(0,60), SerializeField] private Vector2 _timeToSneeze;
        [SerializeField] private float _previousSneezeTime;
        [SerializeField] private float _sneezeTime;
        private float _currentTimeToSneeze;
        private bool _justSneeze;
        private AudioSource audioSource;
        public bool JustSneeze => _justSneeze;
        
        private void Start()
        {
            GetRandomTime();
            audioSource = GetComponent<AudioSource>();
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
            _sneezePreview.SetActive(true);
            yield return new WaitForSeconds(_sneezeTime);
            _sneezePreview.SetActive(false);
            _animator.SetBool(Sneeze, false);
            _justSneeze = false;
            audioSource.Play();
        }
        
        private void GetRandomTime()
        {
            _currentTimeToSneeze = Random.Range(_timeToSneeze.x, _timeToSneeze.y);
        }
    }
}
