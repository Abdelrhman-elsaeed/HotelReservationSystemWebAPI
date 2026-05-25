using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.ReservationRoom
{
    public class ReservationRoomVM
    {
        public int ID { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal? PricePerNight { get; set; }
        public List<int> GuestIds { get; set; } = new();
    }
}
