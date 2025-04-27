namespace Inventory.Domain.Exceptions.Repository
{
    public class FailedToDelete_Exception(string entityName, string value, Exception ex) : Exception($"failed_to_delete_{entityName}:{value}", ex)
    {
        public string EntityName { get; } = entityName;
        public string Value { get; } = value;
    }
}
