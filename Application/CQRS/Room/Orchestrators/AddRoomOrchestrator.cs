using Application.CQRS.Facility.Command;
using Application.CQRS.Room.Command;
using Application.CQRS.RoomFacility.Command;
using Application.CQRS.RoomPicture.Command;
using Application.CQRS.RoomType.Command;
using Application.DTOS;
using Application.DTOS.Facility;
using Application.DTOS.Room;
using Application.DTOS.RoomFacility;
using Application.DTOS.RoomPicture;
using Application.DTOS.RoomType;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;


namespace Application.CQRS.Room.Orchestrators
{
    public sealed record AddRoomOrchestrator(
        AddRoomTypeDto type, 
        AddRoomDetailsDto details, 
        AddFacilityDto facility, 
        List<FileUploadDto> Pictures) : IRequest<ResponseViewModel<bool>>;

    public class AddRoomOrchestratorHandler : IRequestHandler<AddRoomOrchestrator, ResponseViewModel<bool>>
    {
        private readonly IMediator _Mediator;

        public AddRoomOrchestratorHandler(IMediator Mediator)
        {
            _Mediator = Mediator;
        }

        public async Task<ResponseViewModel<bool>> Handle(AddRoomOrchestrator request, CancellationToken cancellationToken)
        {
            // 1. Add Room Type
            var roomTypeCommand = new AddRoomTypeCommand(request.type);
            var roomTypeResult = await _Mediator.Send(roomTypeCommand, cancellationToken);
            
            if (!roomTypeResult.IsSuccess)
                throw new BusinessException(Enum.ErrorCode.AddRoomTypeFail, roomTypeResult.Message);

            // 2. Add Room Details
            request.details.RoomTypeId = roomTypeResult.Data.ID;
            var roomDetailsCommand = new AddRoomDetailsCommand(request.details);
            var roomDetailsResult = await _Mediator.Send(roomDetailsCommand, cancellationToken);
            
            if (!roomDetailsResult.IsSuccess)
                throw new BusinessException(Enum.ErrorCode.AddRoomDetailsFail, roomDetailsResult.Message);

            // 3. Add Facility
            var facilityCommand = new AddFacilityCommand(request.facility);
            var facilityResult = await _Mediator.Send(facilityCommand, cancellationToken);
            

            if (!facilityResult.IsSuccess)
                throw new BusinessException(Enum.ErrorCode.UnExpectedError, facilityResult.Message);

            // 5. Assign Facility to Room
            var assignFacilityDto = new AssignFacilityToRoomDto
            {
                RoomId = roomDetailsResult.Data.ID,
                FacilityId = facilityResult.Data.ID
            };

            var assignFacilityCommand = new AssignFacilityToRoomCommand(assignFacilityDto);
            var assignResult = await _Mediator.Send(assignFacilityCommand, cancellationToken);

            if (!assignResult.IsSuccess)
                throw new BusinessException(Enum.ErrorCode.AssignFacilityToRoomFail, assignResult.Message);

            // 5. Upload Pictures
            if (request.Pictures != null && request.Pictures.Any())
            {

                var uploadPicturesCommand = new UploadRoomPicturesCommand(roomDetailsResult.Data.ID, request.Pictures);
                var picturesResult = await _Mediator.Send(uploadPicturesCommand, cancellationToken);

                if (!picturesResult.IsSuccess)      
                    throw new BusinessException(Enum.ErrorCode.UnExpectedError, picturesResult.Message);
            }

            return ResponseViewModel<bool>.Success(true, "Room and all related details added successfully.");
        }
    }
}
