using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Facility;
using Application.DTOS.RoomFacility;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.RoomFacility.Command
{
    public sealed record UpdateRoomFacilityCommand(UpdateRoomFacilityDto model) : IRequest<ResponseViewModel<UpdateRoomFacilityDto>>;

    public class UpdateRoomFacilityCommandHandler : IRequestHandler<UpdateRoomFacilityCommand, ResponseViewModel<UpdateRoomFacilityDto>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.RoomFacility> _Repository;

        public UpdateRoomFacilityCommandHandler(IRepository<Domain.Entities.RoomManagement.RoomFacility> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<UpdateRoomFacilityDto>> Handle(UpdateRoomFacilityCommand request, CancellationToken cancellationToken)
        {
            // 1. Verify if the association link ID exists
            var isExist = await _Repository.CheckExistsByIDAsync(request.model.ID, cancellationToken);

            if (!isExist)
            {
                return ResponseViewModel<UpdateRoomFacilityDto>.Failure(Enum.ErrorCode.RoomFacilityNotExist, "Room Facility not found");
            }

            // 2. Validate if they are already mapped to each other via a different ID link
            var isAssignBefore = await _Repository.CheckExistsByConditionAsync(
                x => x.RoomId == request.model.RoomId && x.FacilityId == request.model.FacilityId && x.ID != request.model.ID, 
                cancellationToken);

            if (isAssignBefore)
            {
                return ResponseViewModel<UpdateRoomFacilityDto>.Failure(Enum.ErrorCode.FacilityAssignedBefore, "This Facility is already assigned to this Room");
            }

            var roomFacilityEntity = request.model.Map<Domain.Entities.RoomManagement.RoomFacility>();

            _Repository.UpdateInclude(roomFacilityEntity, 
                nameof(Domain.Entities.RoomManagement.RoomFacility.RoomId), 
                nameof(Domain.Entities.RoomManagement.RoomFacility.FacilityId));

            var isSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!isSaved)
            {
                return ResponseViewModel<UpdateRoomFacilityDto>.Failure(Enum.ErrorCode.UpdateRoomFacilityFail, "Room Facility fail to update");
            }

            return ResponseViewModel<UpdateRoomFacilityDto>.Success(roomFacilityEntity.Map<UpdateRoomFacilityDto>(), "Room Facility updated successfully");
        }
    }
}
