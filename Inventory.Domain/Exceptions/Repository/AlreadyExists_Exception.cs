namespace Inventory.Domain.Exceptions.Repository
{
    public class AlreadyExists_Exception : Exception
    {
        public string EntityName { get; }
        public string Value { get; }

        public AlreadyExists_Exception(string entityName, string value)
            : base($"{entityName}_already_exists:{value}")
        {
            EntityName = entityName;
            Value = value;
        }
    }
}
