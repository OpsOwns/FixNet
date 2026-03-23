namespace FixNet.Infrastructure.Auth;

public enum IdentityProviderErrorCode
{
    Unknown = 0,
    Conflict,
    NotFound,
    Unauthorized,
    Unavailable
}