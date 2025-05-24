using LAGS.Managers.Pub;
using UnityEngine;

namespace LAGS
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private RectTransform _rotateImage;

        private float _maxValue;

        private void Start()
        {
            _maxValue = PubManager.Instance.DayDuration;
        }

        private void Update()
        {
            var current = PubManager.Instance.CurrentTime;
            var progress = Mathf.Clamp01(current / _maxValue);
            var angle = progress * 360f;
            
            if (!float.IsNaN(angle) && !float.IsInfinity(angle))
            {
                _rotateImage.eulerAngles = new Vector3(0, 0, -angle);
            }
        }
    }
}
