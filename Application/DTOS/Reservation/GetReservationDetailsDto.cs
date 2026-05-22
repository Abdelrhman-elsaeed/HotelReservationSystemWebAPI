using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Reservation
{
    public class GetReservationDetailsDto
    {
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? SpecialRequest { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReservationRoomDto> Rooms { get; set; } = new();
    }
}
