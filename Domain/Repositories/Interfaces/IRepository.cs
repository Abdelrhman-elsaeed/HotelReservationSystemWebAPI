namespace Domain.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
        Task<T> GetByIDAsync(int id);
        Task<bool> CheckExistsByIDAsync(int id);
        Task<bool> CheckExistsByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateIncludeAsync(T entity, params string[] properties);
        Task<bool> SoftDeleteAsync(T entity);
        Task<bool> SaveChangesAsync();
        Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate);
       
    }
}
