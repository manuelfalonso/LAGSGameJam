using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LAGS
{
    public class DayProgressionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _fillProgressUI;

        //[Header("Settings")]
        //[SerializeField] private Gradient _progressColor;

        [Header("Events")]
        public UnityEvent DayProgressFinished = new();

        private float _progress = 0f;
        private bool _inProgress;

        private void Start()
        {
            Setup();
        }

        private void Update()
        {
            if (_inProgress == false) return;

            _progress += Time.deltaTime / GameManager.Instance.Data.DayDurationInSeconds;
            _fillProgressUI.fillAmount = Mathf.Clamp01(_progress);
            //_fillProgressUI.color = _progressColor.Evaluate(_progress);

            if (_progress >= 1f)
            {
                _progress = 0f;
                _inProgress = false;
                DayProgressFinished.Invoke();
            }
        }
        
        public void StartProgress()
        {
            _inProgress = true;
        }

        public void StopProgress()
        {
            _inProgress = false;
        }

        private void Setup()
        {
            if (_fillProgressUI == null)
            {
                Debug.LogError("Fill Progress UI is not set in DayProgressionUI", this);
                return;
            }
            
            _fillProgressUI.fillAmount = 0f;
            _inProgress = false;
        }
    }
}
