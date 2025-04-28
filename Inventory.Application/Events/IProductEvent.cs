namespace inventory_exchange
{
    public interface IProductEvent
    {
        public string TraceId { get; set; }
        public string EventId { get; set; } 
        public int ProductId { get; set; }
    }
}
