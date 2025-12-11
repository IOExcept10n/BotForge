using System.Xml;
using BotForge.Messaging;
using BotForge.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence.Repositories;

internal class BotUserRepository(BotForgeDbContext context) : Repository<BotForgeDbContext, Guid, BotUser>(context), IBotUserRepository
{
    private const int ConcurrencyRetryCount = 1;

    public virtual async Task<BotUser?> GetByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(u => u.Role).Include(u => u.State)
            .FirstOrDefaultAsync(u => u.PlatformUserId == platformId, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<BotUser?> GetByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        if (discriminator == 0)
        {
            return await DbSet.Include(u => u.Role).Include(u => u.State)
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken).ConfigureAwait(false);
        }

        return await DbSet.Include(u => u.Role).Include(u => u.State)
            .FirstOrDefaultAsync(u => u.Username == username && u.Discriminator == discriminator, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<BotUser> GetOrRegisterAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        BotUser? botUser;
        // Try by platform id first
        if (user.Id != 0)
        {
            botUser = await GetByPlatformIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
            if (botUser != null)
            {
                return await SaveWithRetryAsync(botUser, user, now,
                    async () => await GetByPlatformIdAsync(user.Id, cancellationToken).ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
            }
        }

        // Then by username + discriminator
        if (!string.IsNullOrWhiteSpace(user.NormalizedName))
        {
            botUser = await GetByUsernameAsync(user.NormalizedName, user.Discriminator, cancellationToken).ConfigureAwait(false);
            if (botUser != null)
            {
                return await SaveWithRetryAsync(botUser, user, now,
                    async () => await GetByUsernameAsync(user.NormalizedName!, user.Discriminator, cancellationToken).ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
            }
        }

        // Create new user
        var newUser = new BotUser
        {
            PlatformUserId = user.Id,
            Username = user.NormalizedName,
            Discriminator = user.Discriminator,
            DisplayName = user.DisplayName ?? string.Empty,
            PreferredLocale = user.PreferredLocale?.Name,
            OriginalLocale = user.PlatformLocale?.Name,
            CreatedAt = now,
            LastSeen = now
        };

        await AddAsync(newUser, cancellationToken).ConfigureAwait(false);
        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // reload including navigations
        var registered = await GetByPlatformIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
        return registered ?? newUser;
    }

    private async Task<BotUser> SaveWithRetryAsync(
        BotUser botUser,
        UserIdentity user,
        DateTimeOffset now,
        Func<Task<BotUser?>> reload,
        CancellationToken cancellationToken)
    {
        int attempts = 0;
        while (true)
        {
            try
            {
                MergeUserData(botUser, user, now);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return botUser;
            }
            catch (DbUpdateConcurrencyException) when (attempts < ConcurrencyRetryCount)
            {
                attempts++;
                // Reload entity and retry
                var fresh = await reload().ConfigureAwait(false);
                if (fresh == null)
                    throw;
                botUser = fresh;
            }
        }
    }

    private static void MergeUserData(BotUser botUser, UserIdentity user, DateTimeOffset now)
    {
        // Update LastSeen
        botUser.LastSeen = now;

        // Merge PlatformUserId if missing
        if (botUser.PlatformUserId is null or 0)
        {
            if (user.Id != 0)
            {
                botUser.PlatformUserId = user.Id;
            }
        }

        // Merge Username if missing or different
        if (string.IsNullOrWhiteSpace(botUser.Username) && !string.IsNullOrWhiteSpace(user.NormalizedName))
        {
            botUser.Username = user.NormalizedName;
        }

        // Merge Discriminator if missing
        if (botUser.Discriminator == 0 && user.Discriminator != 0)
        {
            botUser.Discriminator = user.Discriminator;
        }

        // Merge DisplayName if missing or empty
        if (string.IsNullOrWhiteSpace(botUser.DisplayName) && !string.IsNullOrWhiteSpace(user.DisplayName))
        {
            botUser.DisplayName = user.DisplayName;
        }

        // Merge PreferredLocale if missing
        if (string.IsNullOrWhiteSpace(botUser.PreferredLocale) && user.PreferredLocale != null)
        {
            botUser.PreferredLocale = user.PreferredLocale.Name;
        }

        // Merge OriginalLocale if missing
        if (string.IsNullOrWhiteSpace(botUser.OriginalLocale) && user.PlatformLocale != null)
        {
            botUser.OriginalLocale = user.PlatformLocale.Name;
        }
    }
}
