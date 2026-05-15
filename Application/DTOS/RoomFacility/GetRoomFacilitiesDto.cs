using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.RoomFacility
{
    public class GetRoomFacilitiesDto
    {
        public int ID { get; set; }
        public int RoomId { get; set; }
        public int FacilityId { get; set; }
    }
}
