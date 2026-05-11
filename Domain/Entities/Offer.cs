
namespace Domain.Entities
{
    public class Offer : BaseEntity
    {
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<RoomOffer> RoomOffers { get; set; }
    }
}
