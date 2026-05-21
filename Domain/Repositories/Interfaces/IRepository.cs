namespace Domain.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAllWithDeleted();

        IQueryable<T> GetAll();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
        IQueryable<T> GetByID(int id);

        Task<T?> GetByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
        Task<T?> GetByIDAsync(int id, CancellationToken cancellationToken = default);
        Task<List<T>> GetAllByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

        Task<bool> CheckExistsByIDAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> CheckExistsByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity,CancellationToken cancellationToken = default);

        void UpdateInclude(T entity, params string[] properties);

        void SoftDelete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default); 
    }
}