namespace Domain.Entities.RoomManagement
{
    public class RoomOffer : BaseEntity
    {
        public int RoomId { get; set; }
        public int OfferId { get; set; }

        // Navigation Properties
        public Room Room { get; set; }
        public Offer Offer { get; set; }
    }
}
