using Domain.Enums;
using System.Linq.Expressions;

namespace Domain.Interfaces.RepositoryBase;

public interface IRepositoryBase<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllIncludingAsync(RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default);
    Task<int> CountAsync(RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetDeletedAsync(CancellationToken cancellationToken = default);
    Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, bool softDelete = true, CancellationToken cancellationToken = default);
    Task<bool> DeleteRangeAsync(IEnumerable<T> entities, bool softDelete = true, CancellationToken cancellationToken = default);
}
