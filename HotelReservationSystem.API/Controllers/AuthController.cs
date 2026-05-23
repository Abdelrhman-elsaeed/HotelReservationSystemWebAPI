using Application.AutoMapper.Profiles;
using Application.CQRS.Auth.Command;
using Application.CQRS.Reservation.Command;
using Application.DTOS;
using Application.DTOS.Reservation;
using Application.DTOS.User;
using Application.ViewModel.Auth;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public AuthController(IMediator mediator)
        {
            _Mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LoginRequestDto = model.Map<LoginRequestDto>();

            var result = await _Mediator.Send(new LoginCommand(LoginRequestDto),cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<LoginResponseVM>.Success(result.Data.Map<LoginResponseVM>(), result.Message));
            else
                return NotFound(ResponseViewModel<LoginResponseVM>.Failure(result.ErrorCode, result.Message));
        }

    }
}
