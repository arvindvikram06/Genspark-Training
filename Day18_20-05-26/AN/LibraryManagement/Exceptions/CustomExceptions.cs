using System;

namespace LibraryManagement.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityType { get; }
        public object Id { get; }

        public EntityNotFoundException(string entityType, object id)
            : base($"{entityType} with ID {id} not found.")
        {
            EntityType = entityType;
            Id = id;
        }
    }

    public class InvalidInputException : Exception
    {
        public InvalidInputException(string message) : base(message)
        {
        }
    }
}
