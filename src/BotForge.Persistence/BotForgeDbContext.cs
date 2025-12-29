using BotForge.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence;

/// <summary>
/// Provides a basic DB context for DB-based persistence in BotForge framework apps.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BotForgeDbContext"/> class with the provided <see cref="DbContextOptions"/>.
/// </remarks>
/// <param name="options">The options for <see cref="DbContext"/>.</param>
public class BotForgeDbContext(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// A set of users logged into bot.
    /// </summary>
    public DbSet<BotUser> Users => Set<BotUser>();

    /// <summary>
    /// A set of roles defined in bot application.
    /// </summary>
    public DbSet<BotRole> Roles => Set<BotRole>();

    /// <summary>
    /// A set of user states info.
    /// </summary>
    public DbSet<UserState> States => Set<UserState>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        // ----- BotUser -----
        modelBuilder.Entity<BotUser>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(u => u.State)
                .WithOne(s => s.User)
                .HasForeignKey<UserState>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(u => u.Username)
                .HasMaxLength(128)
                .IsRequired(false);

            entity.HasIndex(u => u.PlatformUserId)
                .IsUnique();

            entity.HasIndex(r => new { r.Username, r.Discriminator })
                .IsUnique();
        });

        // ----- BotRole -----
        modelBuilder.Entity<BotRole>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Name)
                .HasMaxLength(128)
                .IsRequired();

            entity.HasIndex(r => r.Name)
                .IsUnique();
        });

        // ----- UserState -----
        modelBuilder.Entity<UserState>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.StateId)
                .HasMaxLength(256)
                .IsRequired();

            // if jsonb is supported, use it
            entity.Property(s => s.StateData)
                .HasColumnType(GetJsonColumnType(Database.ProviderName));
        });
    }

    private static string GetJsonColumnType(string? provider)
    {
        if (provider?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true)
            return "jsonb";
        if (provider?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) == true)
            return "nvarchar(max)";
        return "text";
    }
}
