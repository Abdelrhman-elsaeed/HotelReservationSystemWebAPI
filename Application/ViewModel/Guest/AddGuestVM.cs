using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Guest
{
    public class AddGuestVM
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "National ID is required")]
        [StringLength(20, ErrorMessage = "National ID cannot exceed 20 characters")]
        [RegularExpression(@"^\d+$", ErrorMessage = "National ID must contain only numbers")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Mobile number must be between 10 and 15 characters")]
        public string MobileNumber { get; set; }
    }
}
