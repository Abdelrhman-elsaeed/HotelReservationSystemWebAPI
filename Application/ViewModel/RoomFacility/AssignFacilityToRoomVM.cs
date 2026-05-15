using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.RoomFacility
{
    public class AssignFacilityToRoomVM
    {
        [Required(ErrorMessage = "Please select a room.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid room selected.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Please select a facility.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid facility selected.")]
        public int FacilityId { get; set; }
    }
}
