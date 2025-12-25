using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Domain.Interfaces.RepositoryBase;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.Persistence.Repositories.RepositoryBase;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EtolieEGDbContext _context;
        private bool _disposed;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(EtolieEGDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepositoryBase<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (_repositories.TryGetValue(type, out var repo))
                return (IRepositoryBase<T>)repo!;

            var repository = new RepositoryBase<T>(_context);
            _repositories[type] = repository;
            return repository;
        }
        public async Task<bool> SaveChangesAsync(bool useTransaction = false, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!useTransaction)
                {
                    var affected = await _context.SaveChangesAsync(cancellationToken);
                    return affected > 0;
                }

                await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var affected = await _context.SaveChangesAsync(cancellationToken);
                    await tx.CommitAsync(cancellationToken);
                    return affected > 0;
                }
                catch
                {
                    await tx.RollbackAsync(cancellationToken);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
