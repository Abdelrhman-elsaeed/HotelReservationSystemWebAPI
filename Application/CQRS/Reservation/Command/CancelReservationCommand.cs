using Application.DTOS;
using Application.Enum;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Reservation.Command
{
    public sealed record CancelReservationCommand(int ReservationId) : IRequest<ResponseViewModel<bool>>;
    public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.ReservationManagement.Reservation> _repository;
        public CancelReservationCommandHandler(IRepository<Domain.Entities.ReservationManagement.Reservation> repository)
        {
            _repository = repository;
        }
        public async Task<ResponseViewModel<bool>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Retrieve the Reservation
            var reservation = await _repository.GetByIDAsync(request.ReservationId, cancellationToken);
            if (reservation == null)
            {
                return ResponseViewModel<bool>.Failure(ErrorCode.ReservationNotFound, message: "Reservation not found.");
            }
            // Step 2: Business Logic Validation
            if (reservation.Status == ReservationStatus.Cancelled)
            {
                return ResponseViewModel<bool>.Failure(ErrorCode.CancelReservationFail, message: "Reservation is already cancelled.");
            }
            // Step 3: Update State
            reservation.Status = ReservationStatus.Cancelled;
            _repository.UpdateInclude(reservation, nameof(Domain.Entities.ReservationManagement.Reservation.Status));

            // Step 4: Persist Changes
            var isSaved = await _repository.SaveChangesAsync(cancellationToken);
            if (!isSaved)
            {
                return ResponseViewModel<bool>.Failure(ErrorCode.CancelReservationFail, message: "Failed to cancel the reservation.");
            }
            // Step 5: Return Success
            return ResponseViewModel<bool>.Success(true, "Reservation cancelled successfully.");
        }
    }
}
