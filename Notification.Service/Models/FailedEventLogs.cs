namespace Notification.Service.Models
{
    public class FailedEventLogs
    {
        public string EventId { get; set; }
        public string EventType { get; set; }
        public string TraceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Payload { get; set; }
        public int Retries { get; set; } // Para contar los reintentos
        public bool SuccessfullyHandled { get; set; } // Se pudo manejar
        public string ErrorDetails { get; set; } // Detalles del error
    }


}
