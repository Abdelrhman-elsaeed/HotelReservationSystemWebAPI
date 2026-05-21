using Application.DTOS;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Room.Command
{
    public sealed record DeleteRoomDetailsCommand(int RoomId) : IRequest<ResponseViewModel<bool>>;

    public class DeleteRoomDetailsCommandHandler : IRequestHandler<DeleteRoomDetailsCommand, ResponseViewModel<bool>>
    {
        private readonly IRoomRepository _roomRepository;

        public DeleteRoomDetailsCommandHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<ResponseViewModel<bool>> Handle(DeleteRoomDetailsCommand request, CancellationToken cancellationToken)
        {
            var RoomEntity = await _roomRepository.GetByIDAsync(request.RoomId, cancellationToken);
            if (RoomEntity is null)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.RoomNotFound, message: "Room not found!");

            _roomRepository.SoftDelete(RoomEntity);

            var IsSaved = await _roomRepository.SaveChangesAsync(cancellationToken);
            if (!IsSaved)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.DeleteRoomDetailsFail, message: "Fail to delete room details");

            return ResponseViewModel<bool>.Success(IsSaved, message: "Room details deleted successfully");

        }
    }
}
