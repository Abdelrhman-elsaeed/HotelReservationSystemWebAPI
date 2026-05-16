using System.ComponentModel.DataAnnotations;

namespace Application.DTOS.Room
{
    public class AddRoomTypeDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
