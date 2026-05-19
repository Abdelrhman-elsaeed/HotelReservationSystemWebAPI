using Application.DTOS;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.RoomType.Queries
{
    public sealed record CheckRoomTypeExistQuery(int RoomTypeId,CancellationToken CancellationToken) : IRequest<ResponseViewModel<bool>>;


    public class CheckRoomTypeExistQueryHandler : IRequestHandler<CheckRoomTypeExistQuery, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.RoomType> _Repository;

        public CheckRoomTypeExistQueryHandler(IRepository<Domain.Entities.RoomManagement.RoomType> Repository)
        {
            _Repository = Repository;
        }
        public async Task<ResponseViewModel<bool>> Handle(CheckRoomTypeExistQuery request, CancellationToken cancellationToken)
        {
            var IsExist = await _Repository.CheckExistsByIDAsync(request.RoomTypeId,cancellationToken);

            if (IsExist)
                return ResponseViewModel<bool>.Success(IsExist, message: "Room Type Is Exist");
            else
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.RoomTypeNotExist, "Room type not exist!");
        }
    }
}
