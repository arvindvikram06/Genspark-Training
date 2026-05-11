namespace NotifyModelLibrary.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(int userId) : base($"User with ID {userId} not found.") { }
    public UserNotFoundException() : base($"No users found"){}
}
