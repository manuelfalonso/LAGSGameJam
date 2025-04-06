using System;
using System.Collections;
using System.Collections.Generic;
using LAGS.Managers.Pub;
using LAGS.Pub;
using SombraStudios.Shared.AI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace LAGS.Clients
{
    public class Client : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _headAnimator;
        [SerializeField] private Transform _head;
        [SerializeField] private Light2D _fov;

        [Header("Basic Settings")]
        [SerializeField] private float _clientSpeed;
        [SerializeField] private float _timeToOrder;
        [SerializeField] private float _eatingTime;
        private float _currentEatingTime;

        [Header("Field of View Settings")]
        [SerializeField] private float _fovAngle;
        [SerializeField] private LayerMask _obstacleMask;

        [Header("Alert Settings")] 
        [SerializeField] private float _focusTime = 2f;
        [SerializeField] private int _ratDetectedReducePoints = 60;
        [SerializeField] private int _ratAlertedReducePoints = 10;
        [SerializeField] private int _puddleDetectedReducePoints = 5;
        [SerializeField] private int _cheffDetectedReducePoints = 5;
        private float _currentFocusTime;
        
        private Vector2 _lastPosition;
        private Plate _plate;
        public Plate Plate => _plate;
        private Table _table;
        private Chair _chair;
        private Cheff _cheff;
        public Chair Chair => _chair;
        private bool _isAlert;
        private bool _isEscaping;
        private bool _isSitting;
        private bool _pointsReduced;
        private bool _isFinishedEating;
        public bool IsFinishedEating => _isFinishedEating;
        private List<Transform> _rats = new();
        private List<Transform> _puddles = new();
        private LineOfSight.IsInSightData _data;

        private void Start()
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.speed = _clientSpeed;
            _currentEatingTime = _eatingTime;
        }

        private void Update()
        {
            RotateClient();
            CheckHazards();
            ClientAlert();
            Eat();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isSitting) { return; }

            RatEnterDetection(other);
            PuddleEnterDetection(other);
            ChefEnterDetection(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!_isSitting) { return; }

            RatExitDetection(other);
            PuddleExitDetection(other);
            ChefExitDetection(other);
        }

        private void RatEnterDetection(Collider2D other)
        {
            if (!other.TryGetComponent<Rat>(out var rat)) { return; }

            if (!_rats.Contains(rat.transform))
            {
                _rats.Add(rat.transform);
            }
        }

        private void PuddleEnterDetection(Collider2D other)
        {
            if (!other.TryGetComponent<Puddle>(out var puddle)) { return; }

            if (!_puddles.Contains(puddle.transform))
            {
                _puddles.Add(puddle.transform);
            }
        }

        private void ChefEnterDetection(Collider2D other)
        {
            if(other.TryGetComponent<Cheff>(out var chef)) { return; }
            
            _cheff = chef;
        }

        private void RatExitDetection(Collider2D other)
        {
            if (!other.TryGetComponent<Rat>(out var rat)) { return; }

            if (_rats.Contains(rat.transform))
            {
                _rats.Remove(rat.transform);
            }
        }

        private void PuddleExitDetection(Collider2D other)
        {
            if (!other.TryGetComponent<Puddle>(out var puddle)) { return; }

            if (_puddles.Contains(puddle.transform))
            {
                _puddles.Remove(puddle.transform);
            }
        }

        private void ChefExitDetection(Collider2D other)
        {
            if(!other.TryGetComponent<Cheff>(out var chef)) { return; }
            
            _cheff = null;
        }

        private void RotateClient()
        {
            if (Vector3.Distance(transform.position, _agent.destination) <= _agent.stoppingDistance)
            {
                if (_isEscaping)
                {
                    PubManager.Instance.RemoveClient(this);
                    Destroy(gameObject);
                    return;
                }
                
                transform.position = _chair.transform.position;
                _agent.isStopped = true;
                _isSitting = true;
                _animator.SetBool("IsSitting", true);
                _headAnimator.SetBool("IsSitting", true);
                SelectDirection();
                _agent.enabled = false;
                _fov.enabled = true;
            }
            
            if(_isSitting) { return; }
            
            Vector2 currentPosition = _animator.transform.position;
            Vector2 deltaPosition = currentPosition - _lastPosition;

            var clampedDeltaX = Mathf.Clamp(deltaPosition.x, -1f, 1f);
            var clampedDeltaY = Mathf.Clamp(deltaPosition.y, -1f, 1f);

            _lastPosition = currentPosition;
            
            _animator.SetFloat("MoveX", clampedDeltaX);
            _animator.SetFloat("MoveY", clampedDeltaY);
        }

        private void SelectDirection()
        {
            switch (_chair.Direction)
            {
                default:
                case ChairDirection.DownLeft:
                    _animator.SetBool("DownLeft",true);
                    _animator.SetBool("DownRight",false);
                    _animator.SetBool("UpLeft",false);
                    _animator.SetBool("UpRight",false);
                    _headAnimator.SetBool("DownLeft",true);
                    _headAnimator.SetBool("DownRight",false);
                    _headAnimator.SetBool("UpLeft",false);
                    _headAnimator.SetBool("UpRight",false);
                    // Rotate the FOV Z axis to 135 degrees
                    _fov.transform.rotation = Quaternion.Euler(0, 0, 135);
                    break;
                case ChairDirection.DownRight:
                    _animator.SetBool("DownLeft",false);
                    _animator.SetBool("DownRight",true);
                    _animator.SetBool("UpLeft",false);
                    _animator.SetBool("UpRight",false);          
                    _headAnimator.SetBool("DownLeft",false);
                    _headAnimator.SetBool("DownRight",true);
                    _headAnimator.SetBool("UpLeft",false);
                    _headAnimator.SetBool("UpRight",false);
                    _fov.transform.rotation = Quaternion.Euler(0, 0, 225);
                    break;
                case ChairDirection.UpLeft:
                    _animator.SetBool("DownLeft",false);
                    _animator.SetBool("DownRight",false);
                    _animator.SetBool("UpLeft",true);
                    _animator.SetBool("UpRight",false);
                    _headAnimator.SetBool("DownLeft",false);
                    _headAnimator.SetBool("DownRight",false);
                    _headAnimator.SetBool("UpLeft",true);
                    _headAnimator.SetBool("UpRight",false);
                    _fov.transform.rotation = Quaternion.Euler(0, 0, 60);
                    break;
                case ChairDirection.UpRight:
                    _animator.SetBool("DownLeft",false);
                    _animator.SetBool("DownRight",false);
                    _animator.SetBool("UpLeft",false);
                    _animator.SetBool("UpRight",true);
                    _headAnimator.SetBool("DownLeft",false);
                    _headAnimator.SetBool("DownRight",false);
                    _headAnimator.SetBool("UpLeft",false);
                    _headAnimator.SetBool("UpRight",true);
                    _fov.transform.rotation = Quaternion.Euler(0, 0, 315);
                    break;
            }
        }
        
        private void CheckHazards()
        {
            if(!_isSitting) { return; }
            
            CheckRatHazard();
            CheckPuddleHazard();
            CheckChefHazard();
        }

        private void CheckRatHazard()
        {
            if (_rats.Count <= 0 || _isAlert) { return; }

            foreach (var rat in _rats)
            {
                var data = new LineOfSight.IsInSightData
                {
                    StartPoint = _head,
                    EndPoint = rat,
                    ObstaclesMask = _obstacleMask,
                    Is2D = true
                };

                if (!LineOfSight.IsInFieldOfViewAndInSight(data, _fovAngle, out var hit)) { continue; }

                if (hit.collider != null)
                { 
                    if(hit.collider.TryGetComponent(out Table table)){ if(table == _table) { continue; } }
                }
                
                _isAlert = true;
                _currentFocusTime = _focusTime;
                _data = data;
                _table.ReducePoints(_ratAlertedReducePoints, Reason.RatAlerted);
                break;
            }
        }

        private void CheckPuddleHazard()
        {
            if (_puddles.Count <= 0) { return; }

            foreach (var puddle in _puddles)
            {
                var data = new LineOfSight.IsInSightData
                {
                    StartPoint = _head,
                    EndPoint = puddle,
                    ObstaclesMask = _obstacleMask,
                    Is2D = true
                };

                if (!LineOfSight.IsInFieldOfViewAndInSight(data, _fovAngle, out var _)) { continue; }

                _table.ReducePoints(_puddleDetectedReducePoints, Reason.PuddleDetected);
                break;
            }
        }

        private void CheckChefHazard()
        {
            if(_cheff is null) { return; }

            var data = new LineOfSight.IsInSightData
            {
                StartPoint = _head,
                EndPoint = _cheff.transform,
                ObstaclesMask = _obstacleMask,
                Is2D = true
            };
            
            if(!LineOfSight.IsInFieldOfViewAndInSight(data, _fovAngle, out var _)) { return; }
            
            if(!_cheff.JustSneeze) { return; }
            
            _table.ReducePoints(_cheffDetectedReducePoints, Reason.ChefDetected);
        }
        
        private void ClientAlert()
        {
            if(!_isAlert || _pointsReduced) { return; }

            if (!LineOfSight.IsInFieldOfViewAndInSight(_data, _fovAngle, out var hits))
            {
                _isAlert = false;
                _headAnimator.SetBool("IsMoving", false);
                return;
            }
            
            var direction = _data.EndPoint.position - _head.transform.position;
            
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _fov.transform.rotation = Quaternion.Euler(0, 0, angle) * Quaternion.Euler(0, 0, -90);
            
            var clampedDeltaX = Mathf.Clamp(direction.x, -1f, 1f);
            var clampedDeltaY = Mathf.Clamp(direction.y, -1f, 1f);
            
            _headAnimator.SetFloat("HeadMoveX", clampedDeltaX);
            _headAnimator.SetFloat("HeadMoveY", clampedDeltaY);
            _headAnimator.SetBool("IsMoving", true);

            _currentFocusTime -= Time.deltaTime;
            if (_currentFocusTime <= 0)
            {
                _pointsReduced = true;
                _table.ReducePoints(_ratDetectedReducePoints, Reason.RatDetected);
            }
        }
        
        public void Escape()
        {
            _chair.Leave();
            _isSitting = false;
            _animator.SetBool("IsSitting", false);
            _headAnimator.SetBool("IsSitting", false);
            _agent.enabled = true;  
            _isEscaping = true;
            _agent.isStopped = false;
            _agent.SetDestination(PubManager.Instance.PubDoors.position);
        }
        
        private void GetPlate()
        {
            var plate = PubManager.Instance.GetRandomPlate();
            plate.GenerateId($"{gameObject.name}_{plate.Name}_{Guid.NewGuid()}");
            _plate = plate;
            _table.AddPlate(_plate);
        }

        private void Eat()
        {
            if (!_isSitting || _plate.Status is not OrderStatus.Delivered || _isEscaping) { return; }

            if (_currentEatingTime > 0)
            {
                _currentEatingTime -= Time.deltaTime;
            }
            else
            {
                _isFinishedEating = true;
            }
        }

        public void UpdatePlateReference(Plate plate)
        {
            _plate = plate;
        }
        
        public void AssignTable(Table table)
        {
            _table = table;
            _table.SitClient(this);
            
            _chair = _table.GetFreeChair();
            _chair.AssignClient(this);
            
            _agent.updateRotation = true;
            _agent.SetDestination(_chair.SittingPosition.position);
            
            StartCoroutine(nameof(WaitToOrder));
        }

        private IEnumerator WaitToOrder()
        {
            yield return new WaitUntil(() => _isSitting);
            yield return new WaitForSeconds(_timeToOrder);
            GetPlate();
        }
    }
}
