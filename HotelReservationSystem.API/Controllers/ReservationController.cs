using Application.AutoMapper.Profiles;
using Application.CQRS.Reservation.Command;
using Application.CQRS.Reservation.Queries;
using Application.CQRS.ReservationRoom.Orchestrators;
using Application.DTOS;
using Application.DTOS.Reservation;
using Application.Enum;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using Domain.Enum;
using HotelReservationSystem.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public ReservationController(IMediator mediator)
        {
            _Mediator = mediator;
        }

        [HttpPost]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.AddReservation })]
        public async Task<IActionResult> AddReservation([FromBody] AddReservationVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ReservationDto = model.Map<AddReservationDto>();

            var result = await _Mediator.Send(new AddReservationCommand(ReservationDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<ReservationReceiptVM>.Success(result.Data.Map<ReservationReceiptVM>(), result.Message));
            else
                return NotFound(ResponseViewModel<ReservationReceiptVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPut]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.UpdateReservation })]
        public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var EntityModel = model.Map<UpdateReservationDto>();

            var result = await _Mediator.Send(new UpdateReservationOrchestratorCommand(EntityModel), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<ReservationReceiptVM>.Success(result.Data.Map<ReservationReceiptVM>(), result.Message));

            if (result.ErrorCode == ErrorCode.ReservationNotFound)
                return NotFound(ResponseViewModel<ReservationReceiptVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<ReservationReceiptVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPut("{ReservationId}")]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.CancelReservation })]
        public async Task<IActionResult> CancelReservation(int ReservationId, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new CancelReservationCommand(ReservationId), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<bool>.Success(result.Data, result.Message));

            if (result.ErrorCode == ErrorCode.ReservationNotFound)
                return NotFound(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));
        }

        [HttpGet("{ReservationId}")]

        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.GetReservation })]
        public async Task<IActionResult> GetReservation(int ReservationId, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new GetReservationByIdQuery(ReservationId), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<GetReservationDetailsVM>.Success(result.Data.Map<GetReservationDetailsVM>(),result.Message));

            if (result.ErrorCode == ErrorCode.ReservationNotFound)
                return NotFound(ResponseViewModel<GetReservationDetailsVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<GetReservationDetailsVM>.Failure(result.ErrorCode, result.Message));
        }

    }
}
