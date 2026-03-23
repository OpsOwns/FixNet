using System.Net;
using System.Net.Http.Json;
using FixNet.Application.Abstractions;
using FixNet.Application.Users.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FixNet.Infrastructure.Auth;

internal sealed class KeycloakIdentityProvider(
    HttpClient httpClient,
    IOptions<KeycloakSettings> settings,
    ILogger<KeycloakIdentityProvider> logger)
    : IExternalIdentityProvider
{
    public async Task<string> CreateUserAsync(CreateIdentityRequest request, CancellationToken ct = default)
    {
        var payload = new
        {
            username = request.Email,
            email = request.Email,
            firstName = request.FirstName,
            lastName = request.LastName,
            enabled = true,
            emailVerified = false,
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

        logger.LogInformation(
            "User created in Keycloak: {Email} → {KeycloakId}",
            request.Email, keycloakUserId);

        return keycloakUserId;
    }

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

        var body = await response.Content.ReadAsStringAsync();

        logger.LogError(
            "Keycloak {Operation} failed for {Context}. Status: {Status}, Body: {Body}",
            operation, context, response.StatusCode, body);


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
            $"Keycloak {operation} failed: {response.StatusCode}. {body}",
            errorCode);
    }


    private string UsersEndpoint() =>
        $"{settings.Value.BaseUrl}/admin/realms/{settings.Value.Realm}/users";

    private string UserEndpoint(string userId) =>
        $"{UsersEndpoint()}/{userId}";
}