using Domain.Primitives;

namespace Domain.Errors
{
    public static class UserError
    {
        public static Error InvalidCredentials => new Error("Invalid login or password");
        public static Error InvalidEmail => new Error("Invalid email");
        public static Error UserExists => new Error("User with this email allready exists");
        public static Error NameRequired => new Error($"Name length must be {GlobalVariables.UserConstants.NameMinLength}-{GlobalVariables.UserConstants.NameMaxLength} symbols");
        public static Error InvalidPasswordLength => new Error($"Password length must be {GlobalVariables.UserConstants.PasswordMinLength}-{GlobalVariables.UserConstants.PasswordMaxLength} symbols");
    }
}
