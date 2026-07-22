namespace FixNet.Application.Features.Users;

public sealed record UserRole(string Value)
{
    public static readonly UserRole Client = new("client");
    public static readonly UserRole Technician = new("technician");
    public static readonly UserRole Manager = new("manager");
    public static readonly UserRole Administrator = new("administrator");
}