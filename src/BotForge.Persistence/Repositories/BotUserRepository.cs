using System.Globalization;
using BotForge.Messaging;
using Microsoft.EntityFrameworkCore;
using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

internal class BotUserRepository(BotForgeDbContext context) : Repository<BotForgeDbContext, Guid, BotUser>(context), IBotUserRepository
{
    public virtual async Task<BotUser?> GetByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(u => u.Role).Include(u => u.State)
            .FirstOrDefaultAsync(u => u.PlatformUserId == platformId, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<BotUser?> GetByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        // Case-insensitive search using invariant culture (use invariant upper-case for comparisons)
        string normalized = username.ToUpperInvariant();

#pragma warning disable CA1862 // Use StringComparison. Should be ignored because EF Core cannot compile this as SQL.
        if (discriminator == 0)
        {
            return await DbSet.Include(u => u.Role).Include(u => u.State)
                .FirstOrDefaultAsync(u => u.Username != null && u.Username.ToUpperInvariant() == normalized, cancellationToken).ConfigureAwait(false);
        }

        return await DbSet.Include(u => u.Role).Include(u => u.State)
            .FirstOrDefaultAsync(u => u.Username != null && u.Username.ToUpperInvariant() == normalized && u.Discriminator == discriminator, cancellationToken).ConfigureAwait(false);
#pragma warning restore CA1862
    }

    public virtual async Task<BotUser> GetOrRegisterAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        BotUser? botUser = null;
        bool needsUpdate = false;
        var now = DateTimeOffset.UtcNow;

        // Try by platform id first
        if (user.Id != 0)
        {
            botUser = await GetByPlatformIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
            if (botUser != null)
            {
                // Merge missing fields
                needsUpdate = MergeUserData(botUser, user, now);
                if (needsUpdate)
                {
                    await UpdateAsync(botUser, cancellationToken).ConfigureAwait(false);
                    await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                return botUser;
            }
        }

        // Then by username + discriminator
        if (!string.IsNullOrWhiteSpace(user.Username))
        {
            botUser = await GetByUsernameAsync(user.Username, user.Discriminator, cancellationToken).ConfigureAwait(false);
            if (botUser != null)
            {
                // Merge missing fields
                needsUpdate = MergeUserData(botUser, user, now);
                if (needsUpdate)
                {
                    await UpdateAsync(botUser, cancellationToken).ConfigureAwait(false);
                    await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                return botUser;
            }
        }

        // Create new user
        var newUser = new BotUser
        {
            Id = Guid.NewGuid(),
            PlatformUserId = user.Id,
            Username = user.Username ?? user.Id.ToString(CultureInfo.InvariantCulture),
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

    private static bool MergeUserData(BotUser botUser, UserIdentity user, DateTimeOffset now)
    {
        bool updated = false;

        // Update LastSeen
        botUser.LastSeen = now;
        updated = true;

        // Merge PlatformUserId if missing
        if (botUser.PlatformUserId == null || botUser.PlatformUserId == 0)
        {
            if (user.Id != 0)
            {
                botUser.PlatformUserId = user.Id;
                updated = true;
            }
        }

        // Merge Username if missing or different
        if (string.IsNullOrWhiteSpace(botUser.Username) && !string.IsNullOrWhiteSpace(user.Username))
        {
            botUser.Username = user.Username;
            updated = true;
        }

        // Merge Discriminator if missing
        if (botUser.Discriminator == 0 && user.Discriminator != 0)
        {
            botUser.Discriminator = user.Discriminator;
            updated = true;
        }

        // Merge DisplayName if missing or empty
        if (string.IsNullOrWhiteSpace(botUser.DisplayName) && !string.IsNullOrWhiteSpace(user.DisplayName))
        {
            botUser.DisplayName = user.DisplayName;
            updated = true;
        }

        // Merge PreferredLocale if missing
        if (string.IsNullOrWhiteSpace(botUser.PreferredLocale) && user.PreferredLocale != null)
        {
            botUser.PreferredLocale = user.PreferredLocale.Name;
            updated = true;
        }

        // Merge OriginalLocale if missing
        if (string.IsNullOrWhiteSpace(botUser.OriginalLocale) && user.PlatformLocale != null)
        {
            botUser.OriginalLocale = user.PlatformLocale.Name;
            updated = true;
        }

        return updated;
    }
}
