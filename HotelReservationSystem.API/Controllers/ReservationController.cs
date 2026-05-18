using Application.AutoMapper.Profiles;
using Application.CQRS.Reservation.Command;
using Application.DTOS;
using Application.DTOS.Reservation;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public ReservationController(IMediator mediator)
        {
            _Mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> AddReservation([FromBody] AddReservationVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ReservationDto = model.Map<AddReservationDto>();

            var result = await _Mediator.Send(new AddReservationCommand(ReservationDto),cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<ReservationReceiptVM>.Success(result.Data.Map<ReservationReceiptVM>(), result.Message));
            else
                return NotFound(ResponseViewModel<ReservationReceiptVM>.Failure(result.ErrorCode, result.Message));
        }

    }
}
