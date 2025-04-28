namespace Notification.Service.Models
{
    public class EventLogs
    {
        public string EventId { get; set; }
        public string EventType { get; set; }  // Esto permitirá identificar el tipo de evento
        public string TraceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Payload { get; set; }  // Aquí se puede almacenar el payload del evento serializado
    }

}
