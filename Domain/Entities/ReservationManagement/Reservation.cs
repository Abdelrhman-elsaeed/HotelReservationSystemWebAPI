
namespace Domain.Entities.ReservationManagement
{
    public class Reservation : BaseEntity
    {
        [ForeignKey("Guest")]
        public int GuestId { get; set; }
        public ReservationStatus Status { get; set; }
        public string SpecialRequest { get; set; }
        public decimal TotalAmount { get; set; }


        public Guest Guest { get; set; }
        public Payment Payment { get; set; }
        public Feedback Feedback { get; set; }
        public ICollection<ReservationRoom> ReservationRooms { get; set; }
    }
}
