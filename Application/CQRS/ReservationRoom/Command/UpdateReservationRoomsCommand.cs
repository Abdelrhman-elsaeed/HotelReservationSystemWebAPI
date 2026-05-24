using Application.CQRS.ReservationRoom.Queries;
using Application.CQRS.Room.Queries;
using Application.DTOS;
using Application.DTOS.Receipt;
using Application.DTOS.RoomReservation;
using Application.Enum;
using Domain.Entities.Guest;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.ReservationRoom.Command
{
    // Responsible for ONLY updating the rooms linked to a reservation
    public sealed record UpdateReservationRoomsCommand(int ReservationId, List<AddReservationRoomDto> Rooms): IRequest<ResponseViewModel<UpdatedRoomsReceiptDto>>;
    public class UpdateReservationRoomsCommandHandler : IRequestHandler<UpdateReservationRoomsCommand, ResponseViewModel<UpdatedRoomsReceiptDto>>
    {
        private readonly IRepository<Domain.Entities.ReservationManagement.ReservationRoom> _reservationRoomRepository;
        private readonly IMediator _mediator;
        public UpdateReservationRoomsCommandHandler(IRepository<Domain.Entities.ReservationManagement.ReservationRoom> reservationRoomRepository,IMediator mediator)
        {
            _reservationRoomRepository = reservationRoomRepository;
            _mediator = mediator;
        }
        public async Task<ResponseViewModel<UpdatedRoomsReceiptDto>> Handle(UpdateReservationRoomsCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate dates
            var roomDateRequests = new List<RoomDateRequest>();

            foreach (var rr in request.Rooms)
            {
                if (rr.CheckOutDate.Date <= rr.CheckInDate.Date)
                    return ResponseViewModel<UpdatedRoomsReceiptDto>.Failure(ErrorCode.InvalidDate, message: $"Invalid dates for Room ID {rr.RoomId}");

                roomDateRequests.Add(new RoomDateRequest(rr.RoomId, rr.CheckInDate, rr.CheckOutDate));
            }

            // 2. Check availability, excluding the current reservation so it doesn't block itself
            var availabilityResponse = await _mediator.Send(new CheckMultipleRoomsAvailabilityQuery(roomDateRequests, request.ReservationId), cancellationToken);

            if (!availabilityResponse.IsSuccess || !availabilityResponse.Data)
                return ResponseViewModel<UpdatedRoomsReceiptDto>.Failure(ErrorCode.RoomNotAvailable, message: availabilityResponse.Message);

            // 3. Clear old rooms (this repository owns this data)
            var oldRooms = await _reservationRoomRepository.GetAllByConditionAsync(
                rr => rr.ReservationId == request.ReservationId, cancellationToken);
            _reservationRoomRepository.DeleteRange(oldRooms);


            // 4. Calculate prices and add new rooms
            decimal grandTotalAmount = 0;
            var roomPrices = new Dictionary<int, decimal>();
            foreach (var roomRequest in request.Rooms)
            {
                int totalNights = (roomRequest.CheckOutDate.Date - roomRequest.CheckInDate.Date).Days;
                var roomPriceResponse = await _mediator.Send(new GetRoomTotalPriceQuery(roomRequest.RoomId), cancellationToken);

                if (!roomPriceResponse.IsSuccess)
                    return ResponseViewModel<UpdatedRoomsReceiptDto>.Failure(roomPriceResponse.ErrorCode, roomPriceResponse.Message);

                decimal roomTotalCost = roomPriceResponse.Data.Value * totalNights;
                grandTotalAmount += roomTotalCost;
                roomPrices[roomRequest.RoomId] = roomPriceResponse.Data.Value;

                var reservationRoom = new Domain.Entities.ReservationManagement.ReservationRoom
                {
                    ReservationId = request.ReservationId,
                    RoomId = roomRequest.RoomId,
                    CheckInDate = roomRequest.CheckInDate,
                    CheckOutDate = roomRequest.CheckOutDate,
                    GuestReservationRooms = roomRequest.RoomGuestIds != null && roomRequest.RoomGuestIds.Any()
                        ? roomRequest.RoomGuestIds.Select(gid => new GuestReservationRoom { GuestId = gid }).ToList() : new List<GuestReservationRoom>()
                };
                await _reservationRoomRepository.AddAsync(reservationRoom, cancellationToken);
            }


            // 5. Save 
            var isSaved = await _reservationRoomRepository.SaveChangesAsync(cancellationToken);
            if (!isSaved)
                return ResponseViewModel<UpdatedRoomsReceiptDto>.Failure(ErrorCode.UpdateReservationFail, message: "Failed to update reservation rooms");


            // 6. Return rooms receipt + the recalculated total
            var receipt = new UpdatedRoomsReceiptDto
            {
                NewTotalAmount = grandTotalAmount,
                Rooms = request.Rooms.Select(r => new RoomReceiptDto
                {
                    RoomId = r.RoomId,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalNights = (r.CheckOutDate.Date - r.CheckInDate.Date).Days,
                    PricePerNight = roomPrices[r.RoomId],
                    RoomTotal = roomPrices[r.RoomId] * (r.CheckOutDate.Date - r.CheckInDate.Date).Days
                }).ToList()
            };
            return ResponseViewModel<UpdatedRoomsReceiptDto>.Success(receipt, "Reservation rooms updated successfully");
        }
    }
}
