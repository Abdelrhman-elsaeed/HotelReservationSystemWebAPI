namespace Domain.Repositories.Interfaces
{
    public interface IRoomRepository : IRepository<Room>
    {
        public Task<decimal?> GetRoomTotalPriceAsync(int RoomId, CancellationToken cancellationToken);
        public Task<IEnumerable<Room>> GetRoomsByPredicateAsync(Expression<Func<Room, bool>>? predicate = null,CancellationToken cancellationToken = default);
        Task<IEnumerable<Room>> GetRoomsByPredicatePagedAsync(Expression<Func<Room, bool>>? predicate,int pageNumber,int pageSize,CancellationToken cancellationToken = default);
    }
}
