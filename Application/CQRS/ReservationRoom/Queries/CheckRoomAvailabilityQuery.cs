using Application.DTOS;
using Domain.Entities.ReservationManagement;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.RoomReservation.Queries
{
    public sealed record CheckRoomAvailabilityQuery(int RoomId, DateTime CheckInDate, DateTime CheckOutDate) : IRequest<ResponseViewModel<bool>>;

    public class CheckRoomAvailabilityQueryHandler : IRequestHandler<CheckRoomAvailabilityQuery, ResponseViewModel<bool>>
    {
        private readonly IRepository<ReservationRoom> _reservationRoomRepository;

        public CheckRoomAvailabilityQueryHandler(IRepository<ReservationRoom> reservationRoomRepository)
        {
            _reservationRoomRepository = reservationRoomRepository;
        }

        public async Task<ResponseViewModel<bool>> Handle(CheckRoomAvailabilityQuery request, CancellationToken cancellationToken)
        {
            // Business Rule: A room is NOT available if there are any existing bookings 
            // where the dates overlap with the requested dates.

            bool hasOverlappingBookings = await _reservationRoomRepository.CheckExistsByConditionAsync(rr => 
                rr.RoomId == request.RoomId &&
                rr.CheckInDate < request.CheckOutDate && 
                rr.CheckOutDate > request.CheckInDate &&
                rr.Deleted == false, cancellationToken);

            bool isAvailable = !hasOverlappingBookings;

            if (!isAvailable)
            {
                 return ResponseViewModel<bool>.Failure(Enum.ErrorCode.RoomNotAvailable, message: $"Room {request.RoomId} is already booked for the selected dates.");
            }

            return ResponseViewModel<bool>.Success(true, message: "Room is available.");
        }
    }
}