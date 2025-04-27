namespace Inventory.Domain.Exceptions.Repository
{
    public class NotFound_Exception(string entityName, string value) : Exception($"{entityName}_not_found:{value}")
    {
        public string EntityName { get; } = entityName;
        public string Value { get; } = value;
    }
}
