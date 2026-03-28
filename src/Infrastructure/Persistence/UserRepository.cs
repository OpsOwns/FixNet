using FixNet.Domain.Users;
using FixNet.Domain.Users.Abstractions;
using FixNet.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixNet.Infrastructure.Persistence;

internal sealed class UserRepository(FixNetDbContext dbContext) : IUserRepository
{
    private readonly DbSet<UserEntity> _userEntities = dbContext.Users;

    public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = new UserEntity()
        {
            Email = user.Email.Value,
            ExternalId = user.ExternalId.Value,
            FirstName = user.FirstName.Value,
            Id = user.Id.Value,
            IsAvailable = user.IsAvailable,
            LastName = user.LastName.Value,
            PhoneNumber = user.PhoneNumber.Value,
            Type = user.Type.ToString()
        };

        await _userEntities.AddAsync(userEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}