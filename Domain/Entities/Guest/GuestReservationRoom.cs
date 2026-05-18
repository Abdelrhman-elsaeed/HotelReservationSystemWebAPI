using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Guest
{
    public class GuestReservationRoom : BaseEntity
    {
        public int GuestId { get; set; }
        public Guest Guest { get; set; }

        public int ReservationRoomId { get; set; }
        public ReservationRoom ReservationRoom { get; set; }

    }
}
