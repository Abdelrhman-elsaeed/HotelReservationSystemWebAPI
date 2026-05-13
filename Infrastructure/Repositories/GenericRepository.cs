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

        public GenericRepository(Context Context)
        {
            _Context = Context;
            _dbSet = _Context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _Context.AddAsync(entity);
            return result.Entity;
        }
        public async Task<bool> SaveChangesAsync()
        {
            var RowSaved = await _Context.SaveChangesAsync();
            return RowSaved > 0;
        }
        public async Task<bool> CheckExistsByIDAsync(int id)
        {
            return await _dbSet.AnyAsync(x => x.ID == id && !x.Deleted);
        }
        public async Task<bool> CheckExistsByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        //--------------------------------------------------------------------------------------

        public void DeleteRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteDeleteRangeAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetByIDAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void SoftDelete(T entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateInclude(T entity, params string[] properties)
        {
            throw new NotImplementedException();
        }
    }
}
