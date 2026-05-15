using Application.AutoMapper.Profiles;
using Application.CQRS.Room.Command;
using Application.DTOS;
using Application.DTOS.Room;
using Application.ViewModel.Room;
using MediatR;
using Microsoft.AspNetCore.Http;
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
