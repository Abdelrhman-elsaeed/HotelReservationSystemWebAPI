using Application.AutoMapper.Profiles;
using Application.CQRS.Auth.Command;
using Application.CQRS.Reservation.Command;
using Application.DTOS;
using Application.DTOS.Auth;
using Application.DTOS.Reservation;
using Application.DTOS.User;
using Application.Enum;
using Application.ViewModel.Auth;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> Login([FromBody] LoginRequestVM model, CancellationToken cancellationToken)
        {
            var LoginRequestDto = model.Map<LoginRequestDto>();

            var result = await _Mediator.Send(new LoginCommand(LoginRequestDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<LoginResponseVM>.Success(result.Data.Map<LoginResponseVM>(), result.Message));

            if (result.ErrorCode == ErrorCode.InvalidPassword || result.ErrorCode == ErrorCode.UserNotFound)
                return Unauthorized(ResponseViewModel<LoginResponseVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<LoginResponseVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPost("{UserId}")]
        public async Task<IActionResult> Logout(int UserId, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new LogoutCommand(UserId), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<bool>.Success(result.Data, result.Message));

            if (result.ErrorCode == ErrorCode.UserNotFound)
                return NotFound(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterVM model, CancellationToken cancellationToken)
        {
            var RegisterDto = model.Map<RegisterDto>();
            var result = await _Mediator.Send(new RegisterCommand(RegisterDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<RegisterResponseVM>.Success(result.Data.Map<RegisterResponseVM>(), result.Message));

            return BadRequest(ResponseViewModel<RegisterResponseVM>.Failure(result.ErrorCode, result.Message));
        }

    }
}
