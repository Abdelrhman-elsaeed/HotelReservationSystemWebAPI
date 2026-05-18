using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Receipt
{
    public class RoomReceiptVM
    {
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal RoomTotal { get; set; }
    }
}
