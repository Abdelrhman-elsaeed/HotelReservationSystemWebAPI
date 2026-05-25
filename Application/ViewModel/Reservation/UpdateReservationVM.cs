using Application.DTOS.RoomReservation;
using Application.ViewModel.ReservationRoom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Reservation
{
    public class UpdateReservationVM
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be a positive integer.")]
        public int ID { get; set; }

        [MaxLength(1000, ErrorMessage = "SpecialRequest cannot exceed 1000 characters.")]
        public string SpecialRequest { get; set; }
        public List<AddReservationRoomVM> Rooms { get; set; }
    }
}
