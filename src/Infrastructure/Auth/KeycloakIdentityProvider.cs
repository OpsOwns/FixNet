using System.Net;
using System.Net.Http.Json;
using FixNet.Application.Base;
using FixNet.Application.Base.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FixNet.Infrastructure.Auth;

internal sealed class KeycloakIdentityProvider(
    HttpClient httpClient,
    IOptions<KeycloakSettings> settings,
    ILogger<KeycloakIdentityProvider> logger)
    : IExternalIdentityProvider
{
    public async Task<string> CreateUserAsync(ExternalIdentity request, CancellationToken ct = default)
    {
        var payload = new
        {
            username = request.Email,
            email = request.Email,
            firstName = request.FirstName,
            lastName = request.LastName,
            enabled = true,
            //TODO for now make it available later we implement activation 
            emailVerified = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = request.Password,
                    temporary = false
                }
            }
        };

        var response = await httpClient.PostAsJsonAsync(UsersEndpoint(), payload, ct);

        await HandleErrorResponse(response, "CreateUser", request.Email);

        var location = response.Headers.Location?.ToString()
                       ?? throw new IdentityProviderException(
                           "Keycloak did not return Location header",
                           IdentityProviderErrorCode.Unknown);

        var keycloakUserId = new Uri(location).Segments.Last();

        logger.LogInformation("User created  {Email}", request.Email);

        return keycloakUserId;
    }

    public async Task SendActivationEmailAsync(string userId, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsync(
            $"{UserEndpoint(userId)}/send-verify-email",
            null,
            ct);

        await HandleErrorResponse(response, "SendActivationEmail", userId);
    }

    public async Task AssignRoleAsync(string userId, string roleName, CancellationToken ct = default)
    {
        var roleResponse = await httpClient.GetAsync($"/admin/realms/{settings.Value.Realm}/roles/{roleName}", ct);
        await HandleErrorResponse(roleResponse, "GetRole", roleName);

        var role = await roleResponse.Content.ReadFromJsonAsync<RoleDto>(ct);

        var assignResponse = await httpClient.PostAsJsonAsync(
            $"/admin/realms/{settings.Value.Realm}/users/{userId}/role-mappings/realm",
            new[] { role },
            ct);

        await HandleErrorResponse(assignResponse, "AssignRole", userId);
    }

    private sealed record RoleDto(string Id, string Name);

    public async Task DeleteUserAsync(
        string externalUserId, CancellationToken ct = default)

    {
        var response = await httpClient.DeleteAsync(UserEndpoint(externalUserId), ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning(
                "User {Id} not found in Keycloak during delete — ignoring",
                externalUserId);

            return;
        }

        await HandleErrorResponse(response, "DeleteUser", externalUserId);
        logger.LogInformation(
            "User deleted from Keycloak: {Id}", externalUserId);
    }

    private async Task HandleErrorResponse(
        HttpResponseMessage response, string operation, string context)
    {
        if (response.IsSuccessStatusCode)
            return;


        if (operation != "CreateUser" && logger.IsEnabled(LogLevel.Debug))
        {
            var body = await response.Content.ReadAsStringAsync();
            logger.LogDebug("Keycloak Error Body: {Body}", body);
        }

        logger.LogError("Keycloak {Operation} failed for {Context}. Status: {StatusCode}", operation, context, response.StatusCode);

        var errorCode = response.StatusCode switch
        {
            HttpStatusCode.Conflict => IdentityProviderErrorCode.Conflict,
            HttpStatusCode.NotFound => IdentityProviderErrorCode.NotFound,
            HttpStatusCode.Unauthorized => IdentityProviderErrorCode.Unauthorized,
            HttpStatusCode.Forbidden => IdentityProviderErrorCode.Unauthorized,
            _ when (int)response.StatusCode >= 500
                => IdentityProviderErrorCode.Unavailable,
            _ => IdentityProviderErrorCode.Unknown
        };

        throw new IdentityProviderException(
            $"Keycloak {operation} failed: {response.StatusCode}",
            errorCode);
    }

    private string UsersEndpoint() =>
        $"/admin/realms/{settings.Value.Realm}/users";

    private string UserEndpoint(string userId) =>
        $"{UsersEndpoint()}/{userId}";
}