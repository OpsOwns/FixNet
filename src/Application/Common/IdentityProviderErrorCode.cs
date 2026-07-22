namespace FixNet.Application.Common;

public enum IdentityProviderErrorCode
{
    Unknown = 0,
    Conflict,
    NotFound,
    Unauthorized,
    Unavailable,
    Forbidden
}