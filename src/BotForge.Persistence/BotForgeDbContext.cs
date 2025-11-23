using BotForge.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence;

public abstract class BotForgeDbContext : DbContext
{
    public DbSet<BotUser> Users => Set<BotUser>();

    public DbSet<UserState> States => Set<UserState>();

    public DbSet<BotRole> Roles => Set<BotRole>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected BotForgeDbContext()
    {
    }

    protected BotForgeDbContext(DbContextOptions options) : base(options)
    {
    }
}
