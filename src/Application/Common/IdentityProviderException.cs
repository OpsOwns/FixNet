namespace FixNet.Application.Common;

public class IdentityProviderException(
    string message,
    IdentityProviderErrorCode errorCode) : Exception(message)
{
    public IdentityProviderErrorCode ErrorCode { get; } = errorCode;
}