using Application.CQRS.Reservation.Command;
using Application.CQRS.Reservation.Queries;
using Application.CQRS.ReservationRoom.Command;
using Application.DTOS;
using Application.DTOS.Receipt;
using Application.DTOS.Reservation;
using Application.Enum;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.ReservationRoom.Orchestrators
{
    public sealed record UpdateReservationOrchestratorCommand(UpdateReservationDto model,CancellationToken CancellationToken) : IRequest<ResponseViewModel<ReservationReceiptDto>>;
    public class UpdateReservationOrchestratorCommandHandler : IRequestHandler<UpdateReservationOrchestratorCommand, ResponseViewModel<ReservationReceiptDto>>
    {
        private readonly IMediator _mediator;
        public UpdateReservationOrchestratorCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<ResponseViewModel<ReservationReceiptDto>> Handle(UpdateReservationOrchestratorCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate: does the reservation exist and is it modifiable?
            var reservationResult = await _mediator.Send(new GetReservationByIdQuery(request.model.ID), cancellationToken);
            if (!reservationResult.IsSuccess)
                throw new BusinessException(reservationResult.ErrorCode, reservationResult.Message);

            var reservationDto = reservationResult.Data;
            if (reservationDto.Status == ReservationStatus.Cancelled.ToString() || reservationDto.Status == ReservationStatus.Rejected.ToString())
                throw new BusinessException(ErrorCode.UpdateReservationFail, "Cannot update a cancelled or rejected reservation");


            // 2. Update basic details
            var detailsResult = await _mediator.Send(new UpdateReservationDetailsCommand(request.model.ID, request.model.SpecialRequest),cancellationToken);
            if (!detailsResult.IsSuccess)
                throw new BusinessException(detailsResult.ErrorCode, detailsResult.Message);

            // 3. Replace rooms, check availability, recalculate price
            var roomsResult = await _mediator.Send(new UpdateReservationRoomsCommand(request.model.ID, request.model.Rooms),cancellationToken);
            if (!roomsResult.IsSuccess)
                throw new BusinessException(roomsResult.ErrorCode, roomsResult.Message);

            // 4. final receipt
            var receipt = new ReservationReceiptDto
            {
                ReservationId = reservationDto.ReservationId,
                CreatedAt = reservationDto.CreatedAt,
                Status = reservationDto.Status,
                TotalAmount = roomsResult.Data.NewTotalAmount,
                GuestId = reservationDto.GuestId,
                SpecialRequest = request.model.SpecialRequest,
                Rooms = roomsResult.Data.Rooms
            };
            return ResponseViewModel<ReservationReceiptDto>.Success(receipt, "Reservation updated successfully");
        }
    }
}
