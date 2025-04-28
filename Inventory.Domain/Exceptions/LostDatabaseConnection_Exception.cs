namespace Inventory.Domain.Exceptions
{
    public class LostDatabaseConnection_Exception(string entityName) : Exception($"{entityName}_cannot_connect_database")
    {
        public string EntityName { get; } = entityName;
    }
}
