using System.Collections.Generic;

namespace LAGS.Pub
{
    public class Order
    {
        public List<Plate> Plates;
        public OrderStatus Status;
        
        public Order (List<Plate> plates)
        {
            Status = OrderStatus.Request;
            Plates = plates;
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
