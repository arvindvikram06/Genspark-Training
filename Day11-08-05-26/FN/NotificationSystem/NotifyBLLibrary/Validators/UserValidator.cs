using NotifyModelLibrary;
using NotifyModelLibrary.Exceptions;

namespace NotifyBLLibrary.Validators;

public static class UserValidator
{
    public static void ValidateUserDetails(string name, string email, string phone)
    {
        ValidateName(name);
        ValidateEmail(email);
        ValidatePhone(phone);
    }

    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name cannot be empty");
        if (name.Length < 2)
            throw new ValidationException("Name must be at least 3 characters long");
        if (name.Length > 100)
            throw new ValidationException("Name cannot exceed 100 characters");
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email cannot be empty");

        if (!email.Contains("@") || !email.Contains("."))
            throw new ValidationException("Invalid email format");
    }

    public static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ValidationException("Phone number cannot be empty.");

        if (!phone.All(char.IsDigit))
            throw new ValidationException("Phone number must contain only digits");

        if (phone.Length != 10)
            throw new ValidationException("Phone number must contain 10 digits");
    }

}
