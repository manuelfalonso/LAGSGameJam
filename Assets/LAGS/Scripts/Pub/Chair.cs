using LAGS.Clients;
using UnityEngine;

namespace LAGS.Pub
{
    public class Chair : MonoBehaviour
    {
        [SerializeField] private Transform _sittingPosition;
        [SerializeField] private SpriteRenderer _foodSprite;
        [SerializeField] private ChairDirection _direction;
        private Client _client;
        public Transform SittingPosition => _sittingPosition;
        public ChairDirection Direction => _direction;
        
        public bool IsEmpty => !_client;

        public void AssignClient(Client client)
        {
            _client = client;
        }
        
        public void SetFoodSprite(Sprite sprite)
        {
            _foodSprite.sprite = sprite;
            _foodSprite.gameObject.SetActive(true);
        }

        public void Leave()
        {
            _client = null;
            _foodSprite.gameObject.SetActive(false);
            _foodSprite.sprite = null;
        }
    }

    public enum ChairDirection
    {
        UpRight,
        UpLeft,
        DownRight,
        DownLeft,
    }
}
