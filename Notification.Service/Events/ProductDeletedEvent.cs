namespace inventory_exchange
{
    public class ProductDeletedEvent : IProductEvent
    {
        public string EventType { get; set; } = "ProductDeleted";  // Tipo de evento
        public string TraceId { get; set; }
        public string EventId { get; set; }
        public int ProductId { get; set; }
    }
}
