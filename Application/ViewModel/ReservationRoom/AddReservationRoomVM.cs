using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.ReservationRoom
{
    public class AddReservationRoomVM
    {
        [Required(ErrorMessage = "Room is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Room ID.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Check-In date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-Out date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }
    }
}
