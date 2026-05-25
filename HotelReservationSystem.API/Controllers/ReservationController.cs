using Application.AutoMapper.Profiles;
using Application.CQRS.Reservation.Command;
using Application.CQRS.ReservationRoom.Orchestrators;
using Application.DTOS;
using Application.DTOS.Reservation;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

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

        [HttpPost]
        public async Task<IActionResult> UpdateReservation(UpdateReservationVM model,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var EntityModel = model.Map<UpdateReservationDto>();

            var result = await _Mediator.Send(new UpdateReservationOrchestratorCommand(EntityModel, cancellationToken));

            if (result.IsSuccess)
                return Ok(ResponseViewModel<ReservationReceiptVM>.Success(result.Data.Map<ReservationReceiptVM>(), result.Message));
            else
                return NotFound(ResponseViewModel<ReservationReceiptVM>.Failure(result.ErrorCode, result.Message));
                
        }

        [HttpPost]
        public async Task<IActionResult> CancelReservation(int ReservationId,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _Mediator.Send(new CancelReservationCommand(ReservationId, cancellationToken));

            if (result.IsSuccess)
                return Ok(ResponseViewModel<bool>.Success(result.Data, result.Message));
            else
                return NotFound(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));
        }

    }
}
