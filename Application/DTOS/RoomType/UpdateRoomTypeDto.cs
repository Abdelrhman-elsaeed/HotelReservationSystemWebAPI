using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.RoomType
{
    public class UpdateRoomTypeDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
