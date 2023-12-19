namespace Qujat.Core.DTOs
{
    public enum OneTimeVerificationTokenType
    {
        SignUpConfirmation,
        PasswordResetConfirmation
    }

    public class CurrentRqUser
    {
        public long UserId { get; set; }
    }
}
