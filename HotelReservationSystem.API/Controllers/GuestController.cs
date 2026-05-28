using Application.AutoMapper.Profiles;
using Application.CQRS.Auth.Command;
using Application.CQRS.Guest.Command;
using Application.CQRS.Guest.Queries;
using Application.CQRS.Reservation.Command;
using Application.DTOS;
using Application.DTOS.Auth;
using Application.DTOS.Guest;
using Application.DTOS.Reservation;
using Application.DTOS.User;
using Application.Enum;
using Application.ViewModel.Auth;
using Application.ViewModel.Guest;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using Domain.Enum;
using HotelReservationSystem.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GuestController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public GuestController(IMediator mediator)
        {
            _Mediator = mediator;
        }

        [HttpPost]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.AddGuest })]
        public async Task<IActionResult> AddGuest([FromBody] AddGuestVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var guestDto = model.Map<AddGuestDto>();
            var result = await _Mediator.Send(new AddGuestCommand(guestDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<AddGuestVM>.Success(result.Data.Map<AddGuestVM>(), result.Message));

            return BadRequest(ResponseViewModel<AddGuestVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPut]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.UpdateGuest })]
        public async Task<IActionResult> UpdateGuest([FromBody] UpdateGuestVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var guestDto = model.Map<UpdateGuestDto>();
            var result = await _Mediator.Send(new UpdateGuestCommand(guestDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<UpdateGuestVM>.Success(result.Data.Map<UpdateGuestVM>(), result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.UpdateGuestFail && result.Message == "Guest not found")
                return NotFound(ResponseViewModel<UpdateGuestVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<UpdateGuestVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.DeleteGuest })]
        public async Task<IActionResult> DeleteGuest(int id, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new DeleteGuestCommand(id), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<bool>.Success(result.Data, result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.GuestNotFound)
                return NotFound(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.GetGuest })]
        public async Task<IActionResult> GetGuest(int id, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new GetGuestQuery(id), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<GetGuestVM>.Success(result.Data.Map<GetGuestVM>(), result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.GetGuestFail && result.Message == "Guest not found")
                return NotFound(ResponseViewModel<GetGuestVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<GetGuestVM>.Failure(result.ErrorCode, result.Message));
        }

    }
}
