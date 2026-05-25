using Application.AutoMapper.Profiles;
using Application.CQRS.Room.Command;
using Application.CQRS.Room.Orchestrators;
using Application.CQRS.Room.Queries;
using Application.CQRS.RoomType.Command;
using Application.DTOS;
using Application.DTOS.Facility;
using Application.DTOS.Room;
using Application.DTOS.RoomPicture;
using Application.DTOS.RoomType;
using Application.ViewModel.Facility;
using Application.ViewModel.Room;
using Application.ViewModel.RoomType;
using HotelReservationSystem.API.Helper.Extension; 
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public RoomController(IMediator mediator)
        {
            _Mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddEntireRoom([FromForm] AddRoomTypeVM typeVM,
            [FromForm] AddRoomDetailsVM detailsVM,
            [FromForm] AddFacilityVM facilityVM, 
            [FromForm] List<IFormFile> Pictures, 
            CancellationToken cancellationToken)
        {
            var picturesDto = Pictures.ToFileUploadDtos();

            var typeDto = typeVM.Map<AddRoomTypeDto>();
            var detailsDto = detailsVM.Map<AddRoomDetailsDto>();
            var facilityDto = facilityVM.Map<AddFacilityDto>();

            var orchestratorCommand = new AddRoomOrchestrator(typeDto, detailsDto, facilityDto, picturesDto);
            var result = await _Mediator.Send(orchestratorCommand, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoomType([FromBody] AddRoomTypeVM model, CancellationToken cancellationToken)
        {
            var RoomTypeDto = model.Map<AddRoomTypeDto>();
            var result = await _Mediator.Send(new AddRoomTypeCommand(RoomTypeDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<AddRoomTypeVM>.Success(result.Data.Map<AddRoomTypeVM>(), result.Message));

            return BadRequest(ResponseViewModel<AddRoomTypeVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms([FromQuery] DateTime? StartDate,
            [FromQuery] DateTime? EndDate,
            [FromQuery] int? RoomTypeId,
            CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new GetAllRoomsQuery(StartDate, EndDate, RoomTypeId), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<IEnumerable<GetRoomVM>>.Success(result.Data.Map<IEnumerable<GetRoomVM>>(), result.Message));

            return BadRequest(ResponseViewModel<IEnumerable<GetRoomVM>>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> AddRoomDetails([FromBody] AddRoomDetailsVM model, CancellationToken cancellationToken)
        {
            var RoomDetailsDto = model.Map<AddRoomDetailsDto>();
            var result = await _Mediator.Send(new AddRoomDetailsCommand(RoomDetailsDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<AddRoomDetailsVM>.Success(result.Data.Map<AddRoomDetailsVM>(), result.Message));

            return BadRequest(ResponseViewModel<AddRoomDetailsVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoomDetails([FromBody] UpdateRoomDetailsVM model, CancellationToken cancellationToken)
        {
            var RoomDetailsDto = model.Map<UpdateRoomDetailsDto>();
            var result = await _Mediator.Send(new UpdateRoomDetailsCommand(RoomDetailsDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<UpdateRoomDetailsVM>.Success(result.Data.Map<UpdateRoomDetailsVM>(), result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.RoomNotFound)
                return NotFound(ResponseViewModel<UpdateRoomDetailsVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<UpdateRoomDetailsVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoomType([FromBody] UpdateRoomTypeVM model, CancellationToken cancellationToken)
        {
            var RoomTypeDto = model.Map<UpdateRoomTypeDto>();
            var result = await _Mediator.Send(new UpdateRoomTypeCommand(RoomTypeDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<UpdateRoomTypeVM>.Success(result.Data.Map<UpdateRoomTypeVM>(), result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.RoomTypeNotExist)
                return NotFound(ResponseViewModel<UpdateRoomTypeVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<UpdateRoomTypeVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpGet("{RoomTypeId}")]
        public async Task<IActionResult> GetRoomType(int RoomTypeId, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new GetRoomTypeQuery(RoomTypeId), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<GetRoomTypeVM>.Success(result.Data.Map<GetRoomTypeVM>(), result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.GetRoomTypeFail)
                return NotFound(ResponseViewModel<GetRoomTypeVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<GetRoomTypeVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpDelete("{RoomId}")]
        public async Task<IActionResult> DeleteEntireRoom(int RoomId, CancellationToken cancellationToken)
        {
            var orchestratorCommand = new DeleteRoomOrchestrator(RoomId);
            var result = await _Mediator.Send(orchestratorCommand, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
