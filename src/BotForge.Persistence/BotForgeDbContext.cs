using BotForge.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence;

public class BotForgeDbContext : DbContext
{
    public DbSet<BotUser> Users => Set<BotUser>();
    public DbSet<BotRole> Roles => Set<BotRole>();
    public DbSet<UserState> States => Set<UserState>();

    public BotForgeDbContext(DbContextOptions options)
        : base(options) { }

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
