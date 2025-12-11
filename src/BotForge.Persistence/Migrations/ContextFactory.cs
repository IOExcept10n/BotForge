using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BotForge.Persistence.Migrations;

public class ContextFactory : IDesignTimeDbContextFactory<BotForgeDbContext>
{
    public BotForgeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BotForgeDbContext>();
        optionsBuilder.UseSqlite("Data Source=botforge.db");

        return new BotForgeDbContext(optionsBuilder.Options);
    }
}
