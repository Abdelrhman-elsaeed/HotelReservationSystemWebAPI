using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Room
{
    public class UpdateRoomDetailsVM
    {
        [Required(ErrorMessage = "Room ID is required")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Room number must be between 1 and 50 characters")]
        public string RoomNumber { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Room type ID must be greater than 0")]
        public int RoomTypeId { get; set; }
    }
}
