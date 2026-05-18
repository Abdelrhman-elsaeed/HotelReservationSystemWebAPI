using Application.DTOS.RoomReservation;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Reservation
{
    public class AddReservationDto
    {
        public int ID { get; set; }
        public int GuestId { get; set; }
        public string? SpecialRequest { get; set; }
        public decimal TotalAmount { get; set; }
        public List<AddReservationRoomDto> Rooms { get; set; }
    }
}
