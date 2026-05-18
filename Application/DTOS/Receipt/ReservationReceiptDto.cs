using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Receipt
{
    public class ReservationReceiptDto
    {
        public int ReservationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int GuestId { get; set; }
        public string? SpecialRequest { get; set; }
        public List<RoomReceiptDto> Rooms { get; set; }
    }
}
