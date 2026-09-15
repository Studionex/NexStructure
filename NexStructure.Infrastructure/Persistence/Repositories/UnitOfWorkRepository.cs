using NexStructure.Domain.Common.Repositories;
using NexStructure.Infrastructure.Persistence.Database;

namespace NexStructure.Infrastructure.Persistence.Repositories;

public class UnitOfWorkRepository(MainContext db):IUnitOfWorkRepository
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}