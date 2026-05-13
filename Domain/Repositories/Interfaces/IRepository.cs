namespace Domain.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
        Task<T?> GetByIDAsync(int id);
        Task<bool> CheckExistsByIDAsync(int id);
        Task<bool> CheckExistsByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        void UpdateInclude(T entity, params string[] properties);
        void SoftDelete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<int> ExecuteDeleteRangeAsync(Expression<Func<T, bool>> predicate);
        Task<bool> SaveChangesAsync(); 
    }
}