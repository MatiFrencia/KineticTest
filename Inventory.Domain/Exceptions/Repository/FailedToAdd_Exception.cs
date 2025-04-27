namespace Inventory.Domain.Exceptions.Repository
{
    public class FailedToAdd_Exception(string entityName, string value, Exception ex) : Exception($"failed_to_add_{entityName}:{value}", ex)
    {
        public string EntityName { get; } = entityName;
        public string Value { get; } = value;
    }
}
