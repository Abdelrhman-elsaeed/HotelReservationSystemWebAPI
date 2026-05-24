using Application.DTOS.RoomReservation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Reservation
{
    public class UpdateReservationDto
    {
        public int ID { get; set; }
        public string SpecialRequest { get; set; }

        // The new set of rooms the user wants to book. Old rooms will be replaced.
        public List<AddReservationRoomDto> Rooms { get; set; }
    }
}
