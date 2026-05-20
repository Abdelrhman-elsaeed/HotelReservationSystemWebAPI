using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Offer
{
    public class AddOfferDto
    {
        public int ID { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
