using Application.AutoMapper.Profiles;
using Application.CQRS.Room.Command;
using Application.CQRS.RoomPicture.Command;
using Application.DTOS;
using Application.DTOS.Room;
using Application.ViewModel.Room;
using HotelReservationSystem.API.Helper.Extension;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RoomPictureController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public RoomPictureController(IMediator mediator)
        {
            _Mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages([FromForm] UploadRoomPicturesVM viewModel,CancellationToken cancellationToken)
        {
            var picturesDtoList = viewModel.Pictures.ToFileUploadDtos();

            var command = new UploadRoomPicturesCommand(viewModel.RoomId, picturesDtoList);

            var result = await _Mediator.Send(command,cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<bool>.Success(result.Data,result.Message));

            return BadRequest(ResponseViewModel<bool>.Success(result.Data, result.Message));
        }
    }
}
