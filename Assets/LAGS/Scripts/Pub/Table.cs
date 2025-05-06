using System.Collections;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using LAGS.Clients;
using LAGS.Managers.Pub;
using LAGS.Player;
using UnityEngine;

namespace LAGS.Pub
{
    public class Table : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<Chair> _chairs = new();
        [SerializeField] private GameObject _orderReadyMarker;
        
        [Header("Rating Settings")]
        [SerializeField] private int _startingPoints = 100;
        [SerializeField] private float _ratingTime = 2f;
        [SerializeField] private int _pointsToReduce = 10;
        [MinMaxSlider(0, 100)] [SerializeField] private Vector2 _threeStarsRating;
        [MinMaxSlider(0, 100)] [SerializeField] private Vector2 _twoStarsRating;
        [MinMaxSlider(0, 100)] [SerializeField] private Vector2 _oneStarsRating;
        [MinMaxSlider(0, 100)] [SerializeField] private Vector2 _zeroStarsRating;
        private int _currentPoints;
        private int _currentPointsToReduce;
        
        [Header("Waiting Settings")]
        [SerializeField] private float _addedTimeToWait = 15f;
        [SerializeField] private int _reducePointsForPlateDelayed = 5;
        [SerializeField] private int _extraReducePoints = 1;
        private float _currentTimeToWait;

        [Header("UI Settings")] 
        [SerializeField] private GameObject _container;
        [SerializeField] private List<SpriteRenderer> _starsPositions = new();
        [SerializeField] private float _timeToHideStars = 2f;
        [SerializeField] private Sprite _goodStarSprite;
        [SerializeField] private Sprite _badStarSprite;
        
        public int ChairsReserved { get => _chairsReserved; set => _chairsReserved = value; }

        private List<Client> _clients = new();
        private bool _isEmpty;
        private List<Plate> _plates = new();
        private Order _order;
        private AudioSource audioSource;
        private int _chairsReserved;

        
        public bool AllClientsFinishedEating => _clients.Count >= 0 && _clients.All(client => client.IsFinishedEating);
        
        private Dictionary<Reason, bool> _reasons = new()
        {
            {Reason.RatAlerted, false},
            {Reason.RatDetected, false},
            {Reason.ChefDetected, false},
            {Reason.PuddleDetected, false},
            {Reason.PlateDelayed, false}
        };

        public bool IsEmpty => _isEmpty;
        private bool CanTakeOrder() 
        {
            var canTake = false;

            foreach (var plate in _order.Plates.Where(plate => plate.Status is OrderStatus.Request))
            {
                canTake = true;
            }

            return canTake;
        }

        private void Start()
        {
            _isEmpty = true;
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            CheckIfClientsAreReady();
            if (_order?.Plates[0].Status is not OrderStatus.InProgress) { return; }
            
            if(_order.AreAllPlatesReady) { return; }

            if (_currentTimeToWait > 0)
            {
                _currentTimeToWait -= Time.deltaTime;
                if (_currentTimeToWait <= 0)
                {
                    _currentPointsToReduce += _reducePointsForPlateDelayed;
                }
            }
            else
            {
                _currentPointsToReduce += _extraReducePoints;
            }
        }

        private void CheckIfClientsAreReady()
        {
            if (!AllClientsFinishedEating) { return; }
            
            if(_clients == null || !_clients.Any()) { return; }
            if(_clients.Count == 0){ return;}
            
            foreach (var client in _clients)
            {
                LeaveTable(client);
                client.Escape();
            }
            _clients.Clear();

            audioSource.Play();
        }

        private void PrepareOrder()
        {
            _order = new Order(_plates, this);
            _orderReadyMarker.SetActive(true);
        }
        
        public void SitClient(Client client)
        {
            InitializeTable();
            _clients.Add(client);
            _isEmpty = false;
        }
        
        private void InitializeTable()
        {
            _currentPoints = _startingPoints;
            _currentPointsToReduce = 0;
            foreach (var reason in _reasons.Keys.ToList())
            {
                _reasons[reason] = false;
            }
        }
        
        public void LeaveTable(Client client)
        {
            _order = null;
            _isEmpty = true;
            _chairsReserved = 0;
            _plates.Clear();
            CalculateReview();
        }
        
        public void AddPlate(Plate plate)
        {
            _plates.Add(plate);

            if (_plates.Count == _clients.Count && _plates.Count == _chairsReserved)
            {
                PrepareOrder();
            }
        }

        public int GetTotalSpaces()
        {
            return _chairs.Count(chair => chair.IsEmpty);
        }

        public void Interact(GameObject interactor)
        {
            if(_order == null || _isEmpty) { return; }
            
            if (!interactor.TryGetComponent(out PlayerOrders player)) { return; }
            
            if(CanTakeOrder())
            {
                if (player.Orders.Contains(_order)) { return; }
                
                if (_order.Plates[0].Status is not OrderStatus.Request) { return; }

                _order.SetOrderStatus(OrderStatus.InProgress);
                player.AddOrder(_order);
                _orderReadyMarker.SetActive(false);
                _currentTimeToWait = _order.Plates.Max(p => p.TimeToPrepare) + _addedTimeToWait;
                return;
            }

            foreach (var plate in _order.Plates)
            {
                if (plate.Id.Equals(player.LeftPlate?.Id))
                {
                    Debug.Log("Delivered");
                    player.RemovePlates(true);
                    plate.ChangeStatus(OrderStatus.Delivered);
                    var client = _clients.Find(p => p.Plate.Id == plate.Id);
                    client.UpdatePlateReference(plate);
                    client.Chair.SetFoodSprite(plate.PlateSprite);
                }
                else if (plate.Id.Equals(player.RightPlate?.Id))
                {
                    player.RemovePlates(false);
                    plate.ChangeStatus(OrderStatus.Delivered);
                    var client = _clients.Find(p => p.Plate.Id == plate.Id);
                    client.UpdatePlateReference(plate);
                    client.Chair.SetFoodSprite(plate.PlateSprite);
                }
            }
            
            if (!_order.AreAllPlatesReady) { return; }

            if (_currentPointsToReduce > 0)
            {
                ReducePoints(_currentPointsToReduce, Reason.PlateDelayed);
            }
        }

        public Chair GetFreeChair()
        {
            return _chairs.FirstOrDefault(chair => chair.IsEmpty);
        }

        public void ReducePoints(int amount, Reason reason)
        {
            if(_reasons[reason]) { return; }
            
            _reasons[reason] = true;
            _currentPoints -= amount;
            
            if(_currentPoints > 0) { return; }

            CalculateReview();
            
            foreach (var client in _clients)
            {
                client.Escape();
            }
        }
        
        private void CalculateReview()
        {
            var goodStars = 0;

            if (_currentPoints >= _threeStarsRating.x && _currentPoints <= _threeStarsRating.y)
            {
                goodStars = 3;
            }
            else if (_currentPoints >= _twoStarsRating.x && _currentPoints <= _twoStarsRating.y)
            {
                goodStars = 2;
            }
            else if (_currentPoints >= _oneStarsRating.x && _currentPoints <= _oneStarsRating.y)
            {
                goodStars = 1;
            }
            else if (_currentPoints >= _zeroStarsRating.x && _currentPoints <= _zeroStarsRating.y)
            {
                goodStars = 0;
            }

            foreach (var star in _starsPositions)
            {
                star.sprite = goodStars > 0 ? _goodStarSprite : _badStarSprite;
                goodStars--;
            }
            
            PubManager.Instance.AddScore(_currentPoints);
            _container.SetActive(true);
            StartCoroutine(nameof(WaitToTurnOffStars));
        }

        private IEnumerator WaitToTurnOffStars()
        {
            yield return new WaitForSeconds(_timeToHideStars);
            _container.SetActive(false);
            
            foreach (var star in _starsPositions)
            {
                star.sprite = null;
            }
        }

        public void InteractExit(GameObject interactor)
        {
            // noop
        }
    }
    
    public enum Reason
    {
        RatAlerted,
        RatDetected,
        ChefDetected,
        PuddleDetected,
        PlateDelayed
    }
}