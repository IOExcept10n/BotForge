using BotForge.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence.Repositories;

public class BotUserRepository(BotForgeDbContext context) : Repository<BotForgeDbContext, Guid, BotUser>(context), IBotUserRepository
{
    public async Task<BotUser?> GetByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default)
        => await Context.Users.FirstOrDefaultAsync(u => u.PlatformUserId == platformId, cancellationToken).ConfigureAwait(false);

    public async Task<BotUser?> GetByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default)
        => await Context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Discriminator == discriminator, cancellationToken).ConfigureAwait(false);

    public async Task<BotUser> GetOrCreateByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default)
    {
        var user = await GetByPlatformIdAsync(platformId, cancellationToken).ConfigureAwait(false);
        if (user != null) return user;
        user = new BotUser
        {
            Id = Guid.NewGuid(),
            PlatformUserId = platformId,
            Username = string.Empty,
            Discriminator = 0,
            PlatformName = string.Empty,
            CreatedAt = DateTimeOffset.UtcNow,
            LastSeen = DateTimeOffset.UtcNow,
            RoleId = 0
        };
        await AddAsync(user, cancellationToken).ConfigureAwait(false);
        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return user;
    }

    public async Task<BotUser> GetOrCreateByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default)
    {
        var user = await GetByUsernameAsync(username, discriminator, cancellationToken).ConfigureAwait(false);
        if (user != null) return user;
        user = new BotUser
        {
            Id = Guid.NewGuid(),
            PlatformUserId = 0,
            Username = username,
            Discriminator = discriminator,
            PlatformName = string.Empty,
            CreatedAt = DateTimeOffset.UtcNow,
            LastSeen = DateTimeOffset.UtcNow,
            RoleId = 0
        };
        await AddAsync(user, cancellationToken).ConfigureAwait(false);
        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return user;
    }
}
