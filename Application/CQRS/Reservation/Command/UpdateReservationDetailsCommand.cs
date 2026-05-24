using Application.DTOS;
using Application.DTOS.Reservation;
using Application.Enum;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Reservation.Command
{
    public sealed record UpdateReservationDetailsCommand(int ReservationId, string? SpecialRequest) : IRequest<ResponseViewModel<bool>>;
    public class UpdateReservationDetailsCommandHandler : IRequestHandler<UpdateReservationDetailsCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.ReservationManagement.Reservation> _reservationRepository;
        public UpdateReservationDetailsCommandHandler(IRepository<Domain.Entities.ReservationManagement.Reservation> reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }
        public async Task<ResponseViewModel<bool>> Handle(UpdateReservationDetailsCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepository.GetByIDAsync(request.ReservationId, cancellationToken);
            if (reservation == null)
                return ResponseViewModel<bool>.Failure(ErrorCode.ReservationNotFound, message: "Reservation not found");

            reservation.SpecialRequest = request.SpecialRequest;
            _reservationRepository.UpdateInclude(reservation,nameof(Domain.Entities.ReservationManagement.Reservation.SpecialRequest));

            var isSaved = await _reservationRepository.SaveChangesAsync(cancellationToken);
            if (!isSaved)
                return ResponseViewModel<bool>.Failure(ErrorCode.UpdateReservationFail, message: "Failed to update reservation details");

            return ResponseViewModel<bool>.Success(true, "Reservation details updated successfully");
        }
    }
}
