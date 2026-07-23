namespace FixNet.Application.Common.Abstractions;

public interface IIdentityContext
{
    string UserId { get; }
    string? Email { get; }
}