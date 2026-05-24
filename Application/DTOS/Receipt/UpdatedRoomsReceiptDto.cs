using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Receipt
{
    public class UpdatedRoomsReceiptDto
    {
        public decimal NewTotalAmount { get; set; }
        public List<RoomReceiptDto> Rooms { get; set; } = new();
    }
}
