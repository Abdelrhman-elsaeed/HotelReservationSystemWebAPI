using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Room;
using Application.ViewModel.Room;
using Domain.Entities.RoomManagement;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Room.Command
{
    public sealed record AddRoomDetailsCommand(AddRoomDetailsDto model) : IRequest<ResponseViewModel<AddRoomDetailsDto>>;

    public class AddRoomDetailsCommandHandler : IRequestHandler<AddRoomDetailsCommand, ResponseViewModel<AddRoomDetailsDto>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.Room> _Repository;
        public AddRoomDetailsCommandHandler(IRepository<Domain.Entities.RoomManagement.Room> Repository)
        {
            _Repository = Repository;
        }
        public async Task<ResponseViewModel<AddRoomDetailsDto>> Handle(AddRoomDetailsCommand request, CancellationToken cancellationToken)
        {
            if (request.model is null)
                return ResponseViewModel<AddRoomDetailsDto>.Failure(Enum.ErrorCode.AddRoomDetailsFail, message: "Room Details cannot be Null.");

            var RoomDetailsEntity = request.model.Map<Domain.Entities.RoomManagement.Room>();
            var result = await _Repository.AddAsync(RoomDetailsEntity,cancellationToken);

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<AddRoomDetailsDto>.Failure(Enum.ErrorCode.AddRoomDetailsFail, message: "Fail to add room details!");

            return ResponseViewModel<AddRoomDetailsDto>.Success(result.Map<AddRoomDetailsDto>(), message: "Room details add successfully");
        }
    }
}
