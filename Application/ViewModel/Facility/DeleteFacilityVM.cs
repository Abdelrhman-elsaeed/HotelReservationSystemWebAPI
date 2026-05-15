using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Facility
{
    public class DeleteFacilityVM
    {
        [Required(ErrorMessage = "Please select a facility to delete.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid facility selected.")]
        public int ID { get; set; }
    }
}
