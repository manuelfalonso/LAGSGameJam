using System.Collections.Generic;
using UnityEngine;

namespace LAGS.Pub
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private List<Transform> _chairs;
        private bool _isEmpty;

        public bool IsEmpty => _isEmpty;

        public Order Order;

        public void SitClient()
        {
            if (!_isEmpty)
            {
                Debug.LogError("Table is not empty");
                return;
            }
            
            _isEmpty = true;
        }
        
        public void LeaveTable()
        {
            if (_isEmpty)
            {
                Debug.LogError("Table is already empty");
                return;
            }
            
            _isEmpty = false;
        }
    }
}