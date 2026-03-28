using System.ComponentModel.DataAnnotations;

namespace FixNet.API.Endpoints.Clients;

public record CreateClientRequest(
    [Required, StringLength(50)] string FirstName,
    [Required, StringLength(50)] string LastName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required, Phone] string PhoneNumber
);