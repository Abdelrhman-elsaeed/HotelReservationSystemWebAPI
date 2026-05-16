using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Room
{
    public class AddRoomDetailsDto
    {
        public int ID { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public int RoomTypeId { get; set; }
    }
}
