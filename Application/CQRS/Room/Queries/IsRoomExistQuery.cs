using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Room.Queries
{
    public sealed record IsRoomExistQuery(int RoomId) : IRequest<bool>;

    public class IsRoomExistQueryHandler : IRequestHandler<IsRoomExistQuery, bool>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.Room> _repository;
        
        public IsRoomExistQueryHandler(IRepository<Domain.Entities.RoomManagement.Room> repository)
        {
            _repository = repository;
        }
        
        public async Task<bool> Handle(IsRoomExistQuery request, CancellationToken cancellationToken)
        {
            return await _repository.CheckExistsByIDAsync(request.RoomId, cancellationToken);
        }
    }
}
