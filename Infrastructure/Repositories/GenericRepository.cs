using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;


namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly Context _Context;
        protected DbSet<T> _dbSet;
        private static readonly string[] ImmutableFieldNames = { nameof(BaseEntity.ID), nameof(BaseEntity.CreatedDate), nameof(BaseEntity.UpdatedDate) };
        public GenericRepository(Context Context)
        {
            _Context = Context;
            _dbSet = _Context.Set<T>();
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _Context.AddAsync(entity,cancellationToken);
            return result.Entity;
        }

        public async Task<bool> CheckExistsByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(expression, cancellationToken);
        }
        public async Task<bool> CheckExistsByIDAsync(int id,CancellationToken cancellationToken = default)
        {
            return await CheckExistsByConditionAsync(x=>x.ID==id, cancellationToken);
        }

        public void SoftDelete(T entity)
        {
            entity.Deleted = true;
            UpdateInclude(entity, nameof(entity.Deleted));
        }
        public void UpdateInclude(T entity, params string[] properties)
        {
            properties = properties.Except(ImmutableFieldNames).ToArray();

            var changeTrackerEntry = _dbSet.Local.FindEntry(entity.ID) ?? _dbSet.Entry(entity);

            // Get the type once
            var entityType = entity.GetType();

            entity.UpdatedDate = DateTime.Now;
            foreach (var entryProperty in changeTrackerEntry.Properties)
            {
                if (properties.Contains(entryProperty.Metadata.Name))
                {
                    entryProperty.CurrentValue = entityType.GetProperty(entryProperty.Metadata.Name)!.GetValue(entity);
                    entryProperty.IsModified = true;
                }
            }
        }


        public IQueryable<T> GetAll()
        {
            return _dbSet.Where(x => x.Deleted == false);
        }
        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return GetAll().Where(expression);
        }
        public IQueryable<T> GetByID(int id)
        {
            return GetByCondition(x => x.ID == id);
        }
        public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
             return await GetByCondition(expression).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<T?> GetByIDAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetByConditionAsync(x => x.ID == id, cancellationToken);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var RowSaved = await _Context.SaveChangesAsync(cancellationToken);
            return RowSaved > 0;
        }

        //--------------------------------------------------------------------------------------

        public void DeleteRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
        public IQueryable<T> GetAllWithDeleted()
        {
            throw new NotImplementedException();
        }
    }
}
