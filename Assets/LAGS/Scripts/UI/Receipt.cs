using DG.Tweening;
using LAGS.Managers.Pub;
using SombraStudios.Shared.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LAGS
{
    public class Receipt : MonoBehaviour
    {
        private const string DAY_SUCCESS = "Day Success";
        private const string DAY_FAILED = "Day Failed";
        
        [SerializeField] private TextMeshProUGUI _dayTxt;
        [SerializeField] private TextMeshProUGUI _minScoreNeeded;
        [SerializeField] private TextMeshProUGUI _totalScore;
        [SerializeField] private TextMeshProUGUI _resultTxt;
        [SerializeField] private Button _nextButton;

        [Header("Tween Config")]
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;

        [Header("UI Config")] 
        [SerializeField] private Sprite _nextSprite;
        [SerializeField] private Sprite _retrySprite;
        [SerializeField] private Image _buttonSprite;
        
        private RectTransform _rectTransform;
        private bool _daySuccess;

        private void Start()
        {
            PubManager.Instance.DayFinished.AddListener(OnDayOver);
            _nextButton.onClick.AddListener(ChangeScene);
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            PubManager.Instance.DayFinished.RemoveListener(OnDayOver);
            _nextButton.onClick.RemoveListener(ChangeScene);
        }

        private void OnDayOver()
        {
            var pub = PubManager.Instance;
            _dayTxt.text = pub.DayName;
            _minScoreNeeded.text = $"{pub.MinScoreToWin}";
            _totalScore.text = $"{pub.CurrentScore}";
            
            _daySuccess = pub.CurrentScore >= pub.MinScoreToWin;
            _resultTxt.text = _daySuccess ? DAY_SUCCESS : DAY_FAILED;
            
            _buttonSprite.sprite = _daySuccess ? _nextSprite : _retrySprite;
            
            _rectTransform.DOMoveY(0f,_duration).SetEase(_ease);
        }
        
        private void ChangeScene()
        {
            if (_daySuccess)
            {
                SceneController.Instance.LoadNextScene();
            }
            else
            {
                SceneController.Instance.Restart();
            }
        }
    }
}
