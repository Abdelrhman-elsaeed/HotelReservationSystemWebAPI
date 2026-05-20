using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Room
{
    public class GetRoomVM
    {
        public int ID { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public int RoomTypeId { get; set; }
    }
}
