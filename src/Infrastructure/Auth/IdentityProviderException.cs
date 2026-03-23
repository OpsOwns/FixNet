namespace FixNet.Infrastructure.Auth;

public class IdentityProviderException : Exception
{
    public IdentityProviderErrorCode ErrorCode { get; }

    public IdentityProviderException(
        string message,
        IdentityProviderErrorCode errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public IdentityProviderException(
        string message,
        IdentityProviderErrorCode errorCode,
        Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}