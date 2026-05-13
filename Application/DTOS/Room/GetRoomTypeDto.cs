using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOS.Room
{
    public class GetRoomTypeDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
