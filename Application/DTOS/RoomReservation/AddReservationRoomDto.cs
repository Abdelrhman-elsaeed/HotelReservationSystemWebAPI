using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.RoomReservation
{
    public class AddReservationRoomDto
    {
        public int ID { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
