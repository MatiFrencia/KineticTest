namespace Inventory.Domain.Exceptions.Repository
{
    public class FailedToUpdate_Exception(string entityName, string value, Exception ex) : Exception($"failed_to_update_{entityName}:{value}", ex)
    {
        public string EntityName { get; } = entityName;
        public string Value { get; } = value;
    }

}
