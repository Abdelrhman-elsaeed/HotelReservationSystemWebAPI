using System.ComponentModel.DataAnnotations;

namespace Application.DTOS.Room
{
    public class AddRoomTypeDto
    {
        [Required(ErrorMessage = "Room type name is required.")]
        [StringLength(100, ErrorMessage = "Room type name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
    }
}
