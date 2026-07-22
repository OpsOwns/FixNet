using FixNet.Application.Common.Abstractions;

namespace FixNet.Application.Features.Users.CreateUser;

public record CreateUserCommand(string FirstName, string LastName, string Email, string? Phone, string Password, UserRole Role) : ICommand;