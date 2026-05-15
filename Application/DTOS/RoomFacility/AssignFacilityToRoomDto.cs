using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Application.DTOS.RoomFacility
{
    public class AssignFacilityToRoomDto
    {
        public int ID { get; set; }
        public int RoomId { get; set; }
        public int FacilityId { get; set; }
    }
}
