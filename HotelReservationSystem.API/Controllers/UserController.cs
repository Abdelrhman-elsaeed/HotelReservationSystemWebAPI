using Application.AutoMapper.Profiles;
using Application.CQRS.Auth.Command;
using Application.CQRS.Reservation.Command;
using Application.CQRS.User.Command;
using Application.DTOS;
using Application.DTOS.Reservation;
using Application.DTOS.User;
using Application.ViewModel.Auth;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using Application.ViewModel.User;
using Domain.Enum;
using HotelReservationSystem.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public UserController(IMediator mediator)
        {
            _Mediator = mediator;
        }

        [HttpPost]

        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.AddUser })]
        public async Task<IActionResult> AddUser(AddUserVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var AddUsertDto = model.Map<AddUserDto>();

            var result = await _Mediator.Send(new AddUserCommand(AddUsertDto),cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<AddUserResponseVM>.Success(result.Data.Map<AddUserResponseVM>(), result.Message));
            else
                return NotFound(ResponseViewModel<AddUserResponseVM>.Failure(result.ErrorCode, result.Message));
        }

    }
}
