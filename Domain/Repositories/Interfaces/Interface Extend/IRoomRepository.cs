namespace Domain.Repositories.Interfaces
{
    public interface IRoomRepository : IRepository<Room>
    {
        public Task<decimal?> GetRoomTotalPriceAsync(int RoomId, CancellationToken cancellationToken);
    }
}
