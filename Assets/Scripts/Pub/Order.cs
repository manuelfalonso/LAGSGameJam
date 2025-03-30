namespace LAGS.Pub
{
    public struct Order
    {
        public Plate PlateSprite;
        public OrderStatus Status;
    }

    public enum OrderStatus
    {
        Request,
        InProgress,
        Ready,
        Delivered
    }
}
