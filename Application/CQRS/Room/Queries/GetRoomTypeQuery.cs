

using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Room;
using Domain.Entities.RoomManagement;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Room.Queries
{
    //request
    public sealed record GetRoomTypeQuery(int id) : IRequest<ResponseViewModel<GetRoomTypeDto>>;

    //request handler
    public class GetRoomTypeQueryHandler : IRequestHandler<GetRoomTypeQuery, ResponseViewModel<GetRoomTypeDto>>
    {
        private readonly IRepository<RoomType> _repository;

        public GetRoomTypeQueryHandler(IRepository<RoomType> repository)
        {
            _repository = repository;
        }
        public async Task<ResponseViewModel<GetRoomTypeDto>> Handle(GetRoomTypeQuery request, CancellationToken cancellationToken)
        {
            var ResultEntity = await _repository.GetByIDAsync(request.id);

            if (ResultEntity is null)
                return ResponseViewModel<GetRoomTypeDto>.Failure(Enum.ErrorCode.GetRoomTypeFail, message: "Room Type not found!");

            var ResultDto =  ResultEntity.Map<GetRoomTypeDto>();
            return ResponseViewModel<GetRoomTypeDto>.Success(ResultDto, message: "Room type retrieved successfully");
        }
    }
}
