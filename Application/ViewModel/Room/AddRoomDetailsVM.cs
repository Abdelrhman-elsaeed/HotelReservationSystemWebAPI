using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Room
{
    public class AddRoomDetailsVM
    {
        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(50, ErrorMessage = "Room number cannot exceed 50 characters.")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

    }
}
