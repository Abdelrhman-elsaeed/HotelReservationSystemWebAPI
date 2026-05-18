
using Application.CQRS.Guest.Queries;
using Application.CQRS.Room.Queries;
using Application.CQRS.RoomReservation.Queries;
using Application.DTOS;
using Application.DTOS.Reservation;
using Domain.Entities.ReservationManagement;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using MediatR;


namespace Application.CQRS.Reservation.Command
{
    public sealed record AddReservationCommand(AddReservationDto model) : IRequest<ResponseViewModel<AddReservationDto>>;

    public class AddReservationCommandHandler : IRequestHandler<AddReservationCommand, ResponseViewModel<AddReservationDto>>
    {
        private readonly IRepository<Domain.Entities.ReservationManagement.Reservation> _Repository;
        // Inject generic repo for ReservationRoom to handle the save if you are not using UnitOfWork. 
        // If your _Repository.AddAsync handles object graphs perfectly, you don't need this.
        private readonly IMediator _Mediator;

        public AddReservationCommandHandler(IRepository<Domain.Entities.ReservationManagement.Reservation> Repository, IMediator mediator)
        {
            _Repository = Repository;
            _Mediator = mediator;
        }

        public async Task<ResponseViewModel<AddReservationDto>> Handle(AddReservationCommand request, CancellationToken cancellationToken)
        {
            // 1. Check guest exist
            var IsGuestExist = await _Mediator.Send(new IsGuestExistQuery(request.model.GuestId), cancellationToken);
            if (!IsGuestExist.IsSuccess)
                return ResponseViewModel<AddReservationDto>.Failure(Application.Enum.ErrorCode.GuestNotFound, message: "Guest is not found");

            // 2. Initialize the main Reservation Entity
            var reservationEntity = new Domain.Entities.ReservationManagement.Reservation
            {
                GuestId = request.model.GuestId,
                SpecialRequest = request.model.SpecialRequest,
                Status = ReservationStatus.Pending, // Default start status
                ReservationRooms = new List<ReservationRoom>()
            };

            decimal grandTotalAmount = 0;

            // 3. Process each requested Room
            foreach (var roomRequest in request.model.Rooms)
            {
                // Validate Dates
                if (roomRequest.CheckOutDate.Date <= roomRequest.CheckInDate.Date)
                    return ResponseViewModel<AddReservationDto>.Failure(Enum.ErrorCode.InvalidDate, message: $"Invalid dates for Room ID {roomRequest.RoomId}");

                // Check Availability
                var availabilityResponse = await _Mediator.Send(new CheckRoomAvailabilityQuery(roomRequest.RoomId, roomRequest.CheckInDate, roomRequest.CheckOutDate), cancellationToken);
                
                if (!availabilityResponse.IsSuccess || !availabilityResponse.Data)
                    return ResponseViewModel<AddReservationDto>.Failure(Application.Enum.ErrorCode.RoomNotAvailable, message: availabilityResponse.Message);
                // ------------------------------------

                int totalNights = (roomRequest.CheckOutDate.Date - roomRequest.CheckInDate.Date).Days;

                // Get the Per-Night price
                var roomPriceResponse = await _Mediator.Send(new GetRoomTotalPriceQuery(roomRequest.RoomId), cancellationToken);
                
                if (!roomPriceResponse.IsSuccess)
                    return ResponseViewModel<AddReservationDto>.Failure(roomPriceResponse.ErrorCode, roomPriceResponse.Message);

                // Accumulate the cost
                decimal roomTotalCostForStay = roomPriceResponse.Data.Value * totalNights;
                grandTotalAmount += roomTotalCostForStay;

                // Add the room to the Reservation graph
                reservationEntity.ReservationRooms.Add(new ReservationRoom
                {
                    RoomId = roomRequest.RoomId,
                    CheckInDate = roomRequest.CheckInDate,
                    CheckOutDate = roomRequest.CheckOutDate
                });
            }

            // 4. Assign Final Server-Calculated Price
            reservationEntity.TotalAmount = grandTotalAmount;

            // 5. Save
            var result = await _Repository.AddAsync(reservationEntity);

            if (result == null)
                return ResponseViewModel<AddReservationDto>.Failure(Enum.ErrorCode.AddReservationFail, message: "Failed to create reservation");

            return ResponseViewModel<AddReservationDto>.Success(request.model, message: "Reservation created successfully");
        }
    }
}
