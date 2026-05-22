using Application.CQRS.Room.Command;
using Application.CQRS.RoomFacility.Command;
using Application.CQRS.RoomPicture.Command;
using Application.DTOS;
using Application.Enum;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;

namespace Application.CQRS.Room.Orchestrators
{
    public sealed record DeleteRoomOrchestrator(int RoomId) : IRequest<ResponseViewModel<bool>>;

    public class DeleteRoomOrchestratorHandler : IRequestHandler<DeleteRoomOrchestrator, ResponseViewModel<bool>>
    {
        private readonly IMediator _mediator;

        public DeleteRoomOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ResponseViewModel<bool>> Handle(DeleteRoomOrchestrator request, CancellationToken cancellationToken)
        {
            // 1. Delete Room Pictures (physical + DB)
            var deletePicturesCommand = new DeleteRoomPicturesCommand(request.RoomId);
            var picturesResult = await _mediator.Send(deletePicturesCommand, cancellationToken);

            if (!picturesResult.IsSuccess)
                throw new BusinessException(ErrorCode.DeleteRoomPicturesFail, picturesResult.Message);

            // 2. Delete Room Facilities
            var deleteFacilitiesCommand = new DeleteAllFacilitiesOfRoomCommand(request.RoomId);
            var facilitiesResult = await _mediator.Send(deleteFacilitiesCommand, cancellationToken);

            if (!facilitiesResult.IsSuccess)
                throw new BusinessException(ErrorCode.DeleteAllFacilitiesOfRoomFail, facilitiesResult.Message);

            // 3. Delete Room Details
            var deleteRoomDetailsCommand = new DeleteRoomDetailsCommand(request.RoomId);
            var roomDetailsResult = await _mediator.Send(deleteRoomDetailsCommand, cancellationToken);

            if (!roomDetailsResult.IsSuccess)
                throw new BusinessException(ErrorCode.DeleteRoomDetailsFail, roomDetailsResult.Message);

            return ResponseViewModel<bool>.Success(true, "Room and all related details deleted successfully.");
        }
    }
}
