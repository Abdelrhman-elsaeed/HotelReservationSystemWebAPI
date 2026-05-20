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
        public async Task<IActionResult> AddEntireRoom([FromForm] AddRoomTypeVM typeVM,[FromForm] AddRoomDetailsVM detailsVM,[FromForm] AddFacilityVM facilityVM,[FromForm] List<IFormFile> Pictures) 
        {
            var picturesDto = Pictures.ToFileUploadDtos();

            var typeDto = typeVM.Map<AddRoomTypeDto>();
            var detailsDto = detailsVM.Map<AddRoomDetailsDto>();
            var facilityDto = facilityVM.Map<AddFacilityDto>();

            var orchestratorCommand = new AddRoomOrchestrator(typeDto, detailsDto, facilityDto, picturesDto);
            var result = await _Mediator.Send(orchestratorCommand);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoomType(AddRoomTypeVM model)
        {
            var RoomTypeDto = model.Map<AddRoomTypeDto>();
            var result = await _Mediator.Send(new AddRoomTypeCommand(RoomTypeDto));

            if (result.IsSuccess)
                return Ok(ResponseViewModel<AddRoomTypeVM>.Success(result.Data.Map<AddRoomTypeVM>(),result.Message));
            else
                return NotFound(ResponseViewModel<AddRoomTypeVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms([FromForm] DateTime? StartDate, [FromForm] DateTime? EndDate, [FromForm] int? RoomTypeId)
        {
            var result = await _Mediator.Send(new GetAllRoomsQuery(StartDate, EndDate, RoomTypeId));

            if (result.IsSuccess)
                return Ok(ResponseViewModel<IEnumerable<GetRoomVM>>.Success(result.Data.Map<IEnumerable<GetRoomVM>>(), result.Message));
            else
                return NotFound(ResponseViewModel<IEnumerable<GetRoomVM>>.Failure(result.ErrorCode, result.Message));
        }
    }
}
