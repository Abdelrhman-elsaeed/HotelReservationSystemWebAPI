using Application.AutoMapper.Profiles;
using Application.CQRS.RoomType.Queries;
using Application.DTOS;
using Application.DTOS.Room;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Room.Command
{
    public sealed record UpdateRoomDetailsCommand(UpdateRoomDetailsDto model) : IRequest<ResponseViewModel<bool>>;


    public class UpdateRoomDetailsCommandHandler : IRequestHandler<UpdateRoomDetailsCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.Room> _Repository;
        private readonly IMediator _Mediator;



        public UpdateRoomDetailsCommandHandler(IRepository<Domain.Entities.RoomManagement.Room> Repository, IMediator Mediator)
        {
            _Repository = Repository;
            _Mediator = Mediator;
        }
        public async Task<ResponseViewModel<bool>> Handle(UpdateRoomDetailsCommand request, CancellationToken cancellationToken)
        {
            // Validate room exist
            var IsRoomExist = await _Repository.CheckExistsByIDAsync(request.model.ID, cancellationToken);
            if (!IsRoomExist)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.RoomNotFound, message: "Room not found");

            // Validate type exist
            var IsRoomTypeExist = await _Mediator.Send(new CheckRoomTypeExistQuery(request.model.RoomTypeId, cancellationToken));
            if (!IsRoomTypeExist.IsSuccess)
                return ResponseViewModel<bool>.Failure(IsRoomTypeExist.ErrorCode, IsRoomTypeExist.Message);

            // Update room

            var RoomDetailsEntity = request.model.Map<Domain.Entities.RoomManagement.Room>();

            _Repository.UpdateInclude(RoomDetailsEntity
                , nameof(Domain.Entities.RoomManagement.Room.RoomNumber)
                , nameof(Domain.Entities.RoomManagement.Room.Description)
                , nameof(Domain.Entities.RoomManagement.Room.RoomTypeId));

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.UpdateRoomDetailsFail, message: "Room details fail to update!");

            return ResponseViewModel<bool>.Success(IsSaved, message: "Room details updated successfully");
        }
    }
}
