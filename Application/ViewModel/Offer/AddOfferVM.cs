
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Offer
{
    public class AddOfferVM
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "Discount Percentage is required.")]
        [Range(0.01, 100.00, ErrorMessage = "Discount Percentage must be between 0.01 and 100.")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
