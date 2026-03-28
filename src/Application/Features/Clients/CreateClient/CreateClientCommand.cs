using FixNet.Application.Base.Abstractions;

namespace FixNet.Application.Features.Clients.CreateClient;

public record CreateClientCommand(string FirstName, string LastName, string Email, string PhoneNumber, string Password) : ICommand;