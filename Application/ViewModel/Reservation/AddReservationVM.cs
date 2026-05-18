using Application.DTOS.RoomReservation;
using Application.ViewModel.ReservationRoom;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Reservation
{
    public class AddReservationVM
    {
        [Required(ErrorMessage = "Guest is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Guest ID.")]
        public int GuestId { get; set; }

        [MaxLength(500, ErrorMessage = "Special request cannot exceed 500 characters.")]
        public string? SpecialRequest { get; set; }

        [Required(ErrorMessage = "At least one room must be provided for the reservation.")]
        [MinLength(1, ErrorMessage = "You must select at least one room to make a reservation.")]
        public List<AddReservationRoomVM> Rooms { get; set; }
    }
}
