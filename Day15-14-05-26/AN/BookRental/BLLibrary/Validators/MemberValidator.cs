using System.Text.RegularExpressions;
using BLLibrary.Exceptions;

namespace BLLibrary.Validators{
    public class MemberValidator{
        public static void ValidateMemberInput(string name,string email, string phoneNumber){
            ValidateEmail(email);
            ValidatePhoneNumber(phoneNumber);
            ValidateName(name);
        }

        public static void ValidateEmail(string email){
            if(!Regex.IsMatch(email, "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$")){
                throw new ValidationException("email is invalid");
            }
        }

        public static void ValidatePhoneNumber(string phoneNumbe){
            if(!Regex.IsMatch(phoneNumbe, "^[0-9]{10}$")){
                throw new ValidationException("phone number is invalid");
            }

        }
        public static void ValidateName(string name){
            if(string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 30){
                throw new ValidationException("name is invalid");
            }
        }

    }
}