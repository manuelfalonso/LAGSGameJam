using System;
using System.Collections;
using LAGS.Managers.Pub;
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
        private Coroutine _sneezeCoroutine;
        public bool JustSneeze => _justSneeze;
        
        private void Start()
        {
            GetRandomTime();
            audioSource = GetComponent<AudioSource>();
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

        private void Update()
        {
            if (PubManager.Instance.IsDayOver) { return; }
            
            if (_currentTimeToSneeze > 0)
            {
                _currentTimeToSneeze -= Time.deltaTime;
            }
            else
            {
                _sneezeCoroutine = StartCoroutine(nameof(CallAboutSneeze));
                GetRandomTime();
            }
        }

        private void DayOver()
        {
            if (_sneezeCoroutine != null)
            {
                StopCoroutine(_sneezeCoroutine);
            }
        }

        private IEnumerator CallAboutSneeze()
        {
            _animator.SetBool(AboutSneeze, true);
            _sneezePreview.SetActive(true);
            yield return new WaitForSeconds(_previousSneezeTime);
            _sneezeCoroutine = StartCoroutine(nameof(SneezeDuration));
        }

        private IEnumerator SneezeDuration()
        {
            _justSneeze = true;
            _animator.SetBool(Sneeze, true);
            _animator.SetBool(AboutSneeze, false);
            _sneezePreview.SetActive(false);
            yield return new WaitForSeconds(_sneezeTime);
            _animator.SetBool(Sneeze, false);
            _justSneeze = false;
            audioSource.Play();
            _sneezeCoroutine = null;
        }
        
        private void GetRandomTime()
        {
            _currentTimeToSneeze = Random.Range(_timeToSneeze.x, _timeToSneeze.y);
        }
    }
}
