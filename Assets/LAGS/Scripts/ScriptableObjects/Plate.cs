using System;
using UnityEngine;

namespace LAGS.Pub
{
    [Serializable]
    public struct Plate : IEquatable<Plate>
    {
        public string Name;
        public Sprite PlateSprite;
        public float TimeToPrepare;
        [HideInInspector] public OrderStatus Status;

        public Plate Tick()
        {
            TimeToPrepare -= Time.deltaTime;
            
            if (TimeToPrepare > 0) { return this; }
            
            TimeToPrepare = 0;
            Status = OrderStatus.Ready;
            return this;
        }
        
        public void ChangeStatus(OrderStatus status)
        {
            Status = status;
        }

        public bool Equals(Plate other)
        {
            return Name == other.Name && Equals(PlateSprite, other.PlateSprite) && TimeToPrepare.Equals(other.TimeToPrepare) && Status == other.Status;
        }

        public override bool Equals(object obj)
        {
            return obj is Plate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, PlateSprite, TimeToPrepare, (int)Status);
        }
    }
}
