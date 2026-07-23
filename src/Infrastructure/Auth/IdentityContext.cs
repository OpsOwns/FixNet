using System.Security.Claims;
using FixNet.Application.Common.Abstractions;
using Microsoft.AspNetCore.Http;

namespace FixNet.Infrastructure.Auth;

internal sealed class IdentityContext(
    IHttpContextAccessor httpContextAccessor) : IIdentityContext
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException();

    public string UserId =>
        User.FindFirstValue("sub")
        ?? throw new UnauthorizedAccessException("User id not found in token.");

    public string? Email =>
        User.FindFirstValue("email");
}