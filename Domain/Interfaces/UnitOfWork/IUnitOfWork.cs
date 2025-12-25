using Domain.Interfaces.RepositoryBase;

namespace Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryBase<T> GetRepository<T>() where T : class;
        Task<bool> SaveChangesAsync(bool useTransaction = false, CancellationToken cancellationToken = default);
    }
}
