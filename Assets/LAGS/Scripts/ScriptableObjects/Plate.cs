using System;
using UnityEngine;

namespace LAGS.Pub
{
    [Serializable]
    public struct Plate
    {
        public string Name;
        public Sprite PlateSprite;
        public float TimeToPrepare;
        [HideInInspector] public OrderStatus Status;

        public void Tick()
        {
            TimeToPrepare -= Time.deltaTime;
            
            if (TimeToPrepare > 0) { return; }
            
            TimeToPrepare = 0;
            Status = OrderStatus.Ready;
        }
    }
}
