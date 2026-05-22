using Application.CQRS.Guest.Queries;
using Application.CQRS.Room.Queries;
using Application.CQRS.ReservationRoom.Queries;
using Application.DTOS;
using Application.DTOS.Receipt;
using Application.DTOS.Reservation;
using Domain.Entities.Guest;
using Domain.Entities.ReservationManagement;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Reservation.Command
{
    public sealed record AddReservationCommand(AddReservationDto model) : IRequest<ResponseViewModel<ReservationReceiptDto>>;

    public class AddReservationCommandHandler : IRequestHandler<AddReservationCommand, ResponseViewModel<ReservationReceiptDto>>
    {
        private readonly IRepository<Domain.Entities.ReservationManagement.Reservation> _Repository;
        private readonly IMediator _Mediator;

        public AddReservationCommandHandler(IRepository<Domain.Entities.ReservationManagement.Reservation> Repository, IMediator mediator)
        {
            _Repository = Repository;
            _Mediator = mediator;
        }
        public async Task<ResponseViewModel<ReservationReceiptDto>> Handle(AddReservationCommand request, CancellationToken cancellationToken)
        {
            // 1. Check guest exist
            var IsGuestExist = await _Mediator.Send(new IsGuestExistQuery(request.model.GuestId), cancellationToken);
            if (!IsGuestExist.IsSuccess)
                return ResponseViewModel<ReservationReceiptDto>.Failure(Application.Enum.ErrorCode.GuestNotFound, message: "Guest not found");

            // 2. Validate dates and prepare multi-room availability request
            var roomDateRequests = new List<RoomDateRequest>();
            foreach (var rr in request.model.Rooms)
            {
                if (rr.CheckOutDate.Date <= rr.CheckInDate.Date)
                    return ResponseViewModel<ReservationReceiptDto>.Failure(Enum.ErrorCode.InvalidDate, message: $"Invalid dates for Room ID {rr.RoomId}");

                roomDateRequests.Add(new RoomDateRequest(rr.RoomId, rr.CheckInDate, rr.CheckOutDate));
            }

            // 3. Check availability for all rooms in a single query
            var availabilityResponse = await _Mediator.Send(new CheckMultipleRoomsAvailabilityQuery(roomDateRequests), cancellationToken);
            if (!availabilityResponse.IsSuccess || !availabilityResponse.Data)
                return ResponseViewModel<ReservationReceiptDto>.Failure(Application.Enum.ErrorCode.RoomNotAvailable, message: availabilityResponse.Message);

            // 4. Initialize the main Reservation Entity
            var reservationEntity = new Domain.Entities.ReservationManagement.Reservation
            {
                GuestId = request.model.GuestId,
                SpecialRequest = request.model.SpecialRequest,
                Status = ReservationStatus.Pending,
                ReservationRooms = new List<Domain.Entities.ReservationManagement.ReservationRoom>()
            };

            decimal grandTotalAmount = 0;
            var roomPrices = new Dictionary<int, decimal>();

            // 5. Process each requested Room (pricing & mapping)
            foreach (var roomRequest in request.model.Rooms)
            {
                int totalNights = (roomRequest.CheckOutDate.Date - roomRequest.CheckInDate.Date).Days;

                // Get the Per-Night price
                var roomPriceResponse = await _Mediator.Send(new GetRoomTotalPriceQuery(roomRequest.RoomId), cancellationToken);

                if (!roomPriceResponse.IsSuccess)
                    return ResponseViewModel<ReservationReceiptDto>.Failure(roomPriceResponse.ErrorCode, roomPriceResponse.Message);

                // Accumulate the cost
                decimal roomTotalCostForStay = roomPriceResponse.Data.Value * totalNights;
                grandTotalAmount += roomTotalCostForStay;

                // Map Room and Guests
                var reservationRoom = new Domain.Entities.ReservationManagement.ReservationRoom
                {
                    RoomId = roomRequest.RoomId,
                    CheckInDate = roomRequest.CheckInDate,
                    CheckOutDate = roomRequest.CheckOutDate,
                    GuestReservationRooms = roomRequest.RoomGuestIds != null && roomRequest.RoomGuestIds.Any()
                        ? roomRequest.RoomGuestIds.Select(gid => new GuestReservationRoom { GuestId = gid }).ToList()
                        : new List<GuestReservationRoom>()
                };
                roomPrices[roomRequest.RoomId] = roomPriceResponse.Data.Value;
                // Add the room to the Reservation graph
                reservationEntity.ReservationRooms.Add(reservationRoom);
            }

            // 6. Assign Final Server-Calculated Price
            reservationEntity.TotalAmount = grandTotalAmount;

            // 7. Save
            var result = await _Repository.AddAsync(reservationEntity, cancellationToken);
            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<ReservationReceiptDto>.Failure(Enum.ErrorCode.AddReservationFail, message: "Failed to create reservation");

            var receipt = new ReservationReceiptDto
            {
                ReservationId = result.ID,
                CreatedAt = result.CreatedDate,
                Status = reservationEntity.Status.ToString(),
                TotalAmount = reservationEntity.TotalAmount,
                GuestId = reservationEntity.GuestId,
                SpecialRequest = reservationEntity.SpecialRequest,
                Rooms = request.model.Rooms.Select((r, index) => new RoomReceiptDto
                {
                    RoomId = r.RoomId,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalNights = (r.CheckOutDate.Date - r.CheckInDate.Date).Days,
                    PricePerNight = roomPrices[r.RoomId],
                    RoomTotal = roomPrices[r.RoomId] * (r.CheckOutDate.Date - r.CheckInDate.Date).Days
                }).ToList()
            };

            return ResponseViewModel<ReservationReceiptDto>.Success(receipt, "Reservation created successfully");
        }
    }
}
