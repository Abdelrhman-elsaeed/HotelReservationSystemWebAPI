using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Facility;
using Application.DTOS.RoomFacility;
using Domain.Entities.RoomManagement;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.RoomFacility.Command
{
    //request
    public sealed record AssignFacilityToRoomCommand(AssignFacilityToRoomDto model) : IRequest<ResponseViewModel<AssignFacilityToRoomDto>>;

    //request handler
    public class AssignFacilityToRoomCommandHandler : IRequestHandler<AssignFacilityToRoomCommand, ResponseViewModel<AssignFacilityToRoomDto>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.RoomFacility> _Repository;
        public AssignFacilityToRoomCommandHandler(IRepository<Domain.Entities.RoomManagement.RoomFacility> Repository)
        {
            _Repository = Repository;
        }
        public async Task<ResponseViewModel<AssignFacilityToRoomDto>> Handle(AssignFacilityToRoomCommand request, CancellationToken cancellationToken)
        {
            var IsExist = await _Repository.CheckExistsByConditionAsync(x=>x.FacilityId==request.model.FacilityId&&x.RoomId==request.model.RoomId, cancellationToken);

            if (IsExist)
                return ResponseViewModel<AssignFacilityToRoomDto>.Failure(Enum.ErrorCode.FacilityAssignedBefore, message: "Facility assigned before to this room");

            var RoomFacilityEntity = request.model.Map<Domain.Entities.RoomManagement.RoomFacility>();

            var RoomFacilityEntityResult = await _Repository.AddAsync(RoomFacilityEntity,cancellationToken);

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<AssignFacilityToRoomDto>.Failure(Enum.ErrorCode.AssignFacilityToRoomFail, message: "Fail to assign facility to this room");

            return ResponseViewModel<AssignFacilityToRoomDto>.Success(RoomFacilityEntityResult.Map<AssignFacilityToRoomDto>(), message: "Facility assigned to room successfully");

        }
    }
}
