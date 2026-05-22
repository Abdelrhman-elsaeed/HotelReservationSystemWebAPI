using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOS;
using Domain.Entities.ReservationManagement;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.ReservationRoom.Queries
{
    public sealed record RoomDateRequest(int RoomId, DateTime CheckInDate, DateTime CheckOutDate);

    public sealed record CheckMultipleRoomsAvailabilityQuery(List<RoomDateRequest> RoomRequests) : IRequest<ResponseViewModel<bool>>;

    public class CheckMultipleRoomsAvailabilityQueryHandler : IRequestHandler<CheckMultipleRoomsAvailabilityQuery, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.ReservationManagement.ReservationRoom> _reservationRoomRepository;

        public CheckMultipleRoomsAvailabilityQueryHandler(IRepository<Domain.Entities.ReservationManagement.ReservationRoom> reservationRoomRepository)
        {
            _reservationRoomRepository = reservationRoomRepository;
        }

        public async Task<ResponseViewModel<bool>> Handle(CheckMultipleRoomsAvailabilityQuery request, CancellationToken cancellationToken)
        {
            if (request.RoomRequests == null || !request.RoomRequests.Any())
                return ResponseViewModel<bool>.Success(true, "No rooms requested.");

            // Extract distinct RoomIds to filter the Db query
            var roomIds = request.RoomRequests.Select(r => r.RoomId).Distinct().ToList();
            
            // Get the bounding dates to minimize data retrieved from the database
            var minCheckInDate = request.RoomRequests.Min(r => r.CheckInDate);
            var maxCheckOutDate = request.RoomRequests.Max(r => r.CheckOutDate);

            // Evaluate database query: 
            // Gets all overlapping reservation rooms for the requested RoomIds in a single SQL query
            var existingBookings = await _reservationRoomRepository.GetAllByConditionAsync(rr =>
                roomIds.Contains(rr.RoomId) &&
                rr.CheckInDate < maxCheckOutDate && 
                rr.CheckOutDate > minCheckInDate &&
                rr.Deleted == false &&
                rr.Reservation.Status != ReservationStatus.Cancelled &&
                rr.Reservation.Status != ReservationStatus.Rejected, 
                cancellationToken);

            // Filter precisely in-memory according to exact pairs of boundaries 
            var unavailableRoomIds = new List<int>();

            foreach (var roomReq in request.RoomRequests)
            {
                var isConflict = existingBookings.Any(rr =>
                    rr.RoomId == roomReq.RoomId &&
                    rr.CheckInDate < roomReq.CheckOutDate && 
                    rr.CheckOutDate > roomReq.CheckInDate);

                if (isConflict)
                {
                    unavailableRoomIds.Add(roomReq.RoomId);
                }
            }

            if (unavailableRoomIds.Any())
            {
                var distinctUnavailableIds = unavailableRoomIds.Distinct();
                return ResponseViewModel<bool>.Failure(Application.Enum.ErrorCode.RoomNotAvailable, 
                    message: $"The following rooms are already booked for the selected dates: {string.Join(", ", distinctUnavailableIds)}.");
            }

            return ResponseViewModel<bool>.Success(true, message: "All requested rooms are available.");
        }
    }
}
