using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.RoomType
{
    public class UpdateRoomTypeVM
    {
        [Required(ErrorMessage = "ID is required to update room type")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Room type name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Room type name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }
    }
}
