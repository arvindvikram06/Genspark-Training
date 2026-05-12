using System;

namespace WordGame.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email) : base($"User with Email {email} not found.") { }
    }
}