namespace NexStructure.Domain.Common.Repositories;

public interface IUnitOfWorkRepository
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
    
}