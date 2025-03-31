using LAGS.Clients;
using UnityEngine;

namespace LAGS.Pub
{
    public class Chair : MonoBehaviour
    {
        [SerializeField] private Transform _sittingPosition;
        private Client _client;
        public Transform SittingPosition => _sittingPosition;
        
        public bool IsEmpty => !_client;

        public void AssignClient(Client client)
        {
            _client = client;
        }
    }
}
