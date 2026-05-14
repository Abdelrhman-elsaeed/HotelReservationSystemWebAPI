using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Facility
{
    public class DeleteFacilityVM
    {
        [Required(ErrorMessage = "facility id is required.")]
        public int ID { get; set; }
        [Required(ErrorMessage = "Facility name is required.")]
        [StringLength(100, ErrorMessage = "Facility name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal Price { get; set; }
    }
}
