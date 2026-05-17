using Application.AutoMapper.Profiles;
using Application.CQRS.Room.Command;
using Application.CQRS.Room.Orchestrators; 
using Application.DTOS;
using Application.DTOS.Facility;
using Application.DTOS.Room;
using Application.DTOS.RoomPicture;
using Application.ViewModel.Facility;
using Application.ViewModel.Room;
using HotelReservationSystem.API.Helper.Extension; 
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
