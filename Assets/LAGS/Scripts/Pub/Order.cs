using System.Collections.Generic;

namespace LAGS.Pub
{
    public class Order
    {
        public List<Plate> Plates;
        public Table Table;
        public bool AreAllPlatesReady => Plates.TrueForAll(plate => plate.Status is OrderStatus.Ready);
        
        public Order (List<Plate> plates, Table table)
        {
            Plates = plates;
            for (var i = 0; i < Plates.Count; i++)
            {
                var plate = Plates[i];
                plate.Status = OrderStatus.Request;
                Plates[i] = plate;
            }
            Table = table;
        }
        
        public void SetOrderStatus(OrderStatus status)
        {
            for (var i = 0; i < Plates.Count; i++)
            {
                var plate = Plates[i];
                plate.Status = status;
                Plates[i] = plate;
            }
        }
    }

    public enum OrderStatus
    {
        Request,
        InProgress,
        Ready,
        Delivered
    }
}
