using BotForge.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence.Repositories;

internal class UserStateRepository(BotForgeDbContext context) : Repository<BotForgeDbContext, Guid, UserState>(context), IUserStateRepository
{
    public async Task<UserState?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await Context.States.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken).ConfigureAwait(false);
}
