using Application.DTOS.Receipt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Receipt
{
    public class ReservationReceiptVM
    {
        public int ReservationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int GuestId { get; set; }
        public string? SpecialRequest { get; set; }
        public List<RoomReceiptVM> Rooms { get; set; }
    }
}
