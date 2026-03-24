namespace FixNet.Infrastructure.Auth;

public class IdentityProviderException(
    string message,
    IdentityProviderErrorCode errorCode) : Exception(message)
{
    public IdentityProviderErrorCode ErrorCode { get; } = errorCode;
}