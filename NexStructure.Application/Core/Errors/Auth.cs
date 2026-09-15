namespace NexStructure.Application.Core.Errors;

public partial class Errors
{
    public static class Auth
    {
        public static Error Forbidden => Error.Forbidden("Auth.Forbidden",
            "You do not have permission to perform this action."
        );

        public static Error NotFound => Error.NotFound("Auth.NotFound",
            "The specified user was not found."
        );
        public static Error InvalidCode => Error.Unauthorized("Auth.InvalidCode",
            "The provided verification code is invalid."
        );

        public static Error ExpiredCode => Error.Unauthorized("Auth.ExpiredCode",
            "The provided verification code has expired."
        );

        public static Error InvalidCredentials => Error.Unauthorized("Auth.InvalidCredentials",
            "The provided credentials are invalid."
        );

        public static Error Unauthorized => Error.Unauthorized("Auth.Unauthorized",
            "You are not authorized to perform this action."
        );

        public static Error TooManyAttempts => Error.Unauthorized("Auth.TooManyAttempts",
            "Too many invalid attempts. Please request a new verification code."
        );
    }
}
