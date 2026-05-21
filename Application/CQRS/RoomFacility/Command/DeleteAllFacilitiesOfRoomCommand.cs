using Application.DTOS;
using Application.Enum;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.RoomFacility.Command
{
    public sealed record DeleteAllFacilitiesOfRoomCommand(int RoomId) : IRequest<ResponseViewModel<bool>>;

    public class DeleteAllFacilitiesOfRoomCommandHandler : IRequestHandler<DeleteAllFacilitiesOfRoomCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.RoomFacility> _repository;

        public DeleteAllFacilitiesOfRoomCommandHandler(IRepository<Domain.Entities.RoomManagement.RoomFacility> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseViewModel<bool>> Handle(DeleteAllFacilitiesOfRoomCommand request, CancellationToken cancellationToken)
        {
            var roomFacilities = await _repository.GetAllByConditionAsync(x => x.RoomId == request.RoomId, cancellationToken);

            if (!roomFacilities.Any())
                return ResponseViewModel<bool>.Failure(ErrorCode.RoomFacilityNotExist, message: "No facilities found for this room.");

            _repository.DeleteRange(roomFacilities);

            var isSaved = await _repository.SaveChangesAsync(cancellationToken);
            if (!isSaved)
                return ResponseViewModel<bool>.Failure(ErrorCode.DeleteAllFacilitiesOfRoomFail, message: "Failed to delete room facilities.");

            return ResponseViewModel<bool>.Success(true, message: "Room facilities deleted successfully.");
        }
    }
}
