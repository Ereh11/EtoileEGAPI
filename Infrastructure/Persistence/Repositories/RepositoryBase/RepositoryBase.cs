using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using Domain.Enums;
using Domain.Interfaces.RepositoryBase;

namespace Infrastructure.Persistence.Repositories.RepositoryBase
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        private static readonly PropertyInfo? _isDeletedProp = typeof(T).GetProperty("IsDeleted");
        private static readonly PropertyInfo? _deletedAtProp = typeof(T).GetProperty("DeletedAt");

        public RepositoryBase(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        #region Query builder
        protected IQueryable<T> Query(RecordStatus status)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (_isDeletedProp == null)
                return query;

            return status switch
            {
                RecordStatus.ActiveOnly => query.Where(e => !EF.Property<bool>(e, "IsDeleted")),
                RecordStatus.DeletedOnly => query.Where(e => EF.Property<bool>(e, "IsDeleted")),
                RecordStatus.All => query,
                _ => query
            };
        }

        #endregion

        #region Read operations

        public virtual async Task<T?> GetByIdAsync(Guid id, RecordStatus status = RecordStatus.ActiveOnly,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
                if (entity == null) return null;

                // If type doesn't have IsDeleted => return entity
                if (_isDeletedProp == null) return entity;

                var isDeleted = (bool?)_isDeletedProp.GetValue(entity) ?? false;

                if (status == RecordStatus.ActiveOnly && isDeleted) return null;
                if (status == RecordStatus.DeletedOnly && !isDeleted) return null;

                return entity;
            }
            catch
            {
                return null;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(RecordStatus status = RecordStatus.ActiveOnly,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Query(status).ToListAsync(cancellationToken);
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllIncludingAsync(RecordStatus status = RecordStatus.ActiveOnly,
            CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var query = Query(status);
                if (includes != null && includes.Length > 0)
                {
                    foreach (var inc in includes)
                        query = query.Include(inc);
                }
                return await query.ToListAsync(cancellationToken);
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber = 1, int pageSize = 10,
            RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var skip = (pageNumber - 1) * pageSize;
                return await Query(status).Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public virtual async Task<int> CountAsync(RecordStatus status = RecordStatus.ActiveOnly, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Query(status).CountAsync(cancellationToken);
            }
            catch
            {
                return 0;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, RecordStatus status = RecordStatus.ActiveOnly,
            CancellationToken cancellationToken = default)
        {
            if (filter == null) return Enumerable.Empty<T>();

            try
            {
                return await Query(status).Where(filter).ToListAsync(cancellationToken);
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public virtual async Task<IEnumerable<T>> GetDeletedAsync(CancellationToken cancellationToken = default)
        {
            if (_isDeletedProp == null) return Enumerable.Empty<T>();

            try
            {
                return await Query(RecordStatus.DeletedOnly).ToListAsync(cancellationToken);
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public virtual async Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (_isDeletedProp == null) return false;

            try
            {
                var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
                if (entity == null) return false;

                var isDeleted = (bool?)_isDeletedProp.GetValue(entity) ?? false;
                if (!isDeleted) return false;

                _isDeletedProp.SetValue(entity, false);
                _deletedAtProp?.SetValue(entity, null);

                _dbSet.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Create / Update / Delete (in-memory only)
        public virtual async Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return false;
            try
            {
                await _dbSet.AddAsync(entity, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null) return false;
            try
            {
                await _dbSet.AddRangeAsync(entities, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return false;
            try
            {
                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Detached)
                    _dbSet.Attach(entity);

                _dbSet.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null) return false;
            try
            {
                foreach (var e in entities)
                {
                    var entry = _context.Entry(e);
                    if (entry.State == EntityState.Detached)
                        _dbSet.Attach(e);
                }
                _dbSet.UpdateRange(entities);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(Guid id, bool softDelete = true, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
                if (entity == null) return false;

                if (softDelete && _isDeletedProp != null)
                {
                    _isDeletedProp.SetValue(entity, true);
                    _deletedAtProp?.SetValue(entity, DateTime.UtcNow);
                    _dbSet.Update(entity);
                }
                else
                {
                    _dbSet.Remove(entity);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> entities, bool softDelete = true,
            CancellationToken cancellationToken = default)
        {
            if (entities == null) return false;
            try
            {
                if (softDelete && _isDeletedProp != null)
                {
                    var now = DateTime.UtcNow;
                    foreach (var entity in entities)
                    {
                        _isDeletedProp.SetValue(entity, true);
                        _deletedAtProp?.SetValue(entity, now);

                        var entry = _context.Entry(entity);
                        if (entry.State == EntityState.Detached)
                            _dbSet.Attach(entity);
                    }
                    _dbSet.UpdateRange(entities);
                }
                else
                {
                    _dbSet.RemoveRange(entities);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
