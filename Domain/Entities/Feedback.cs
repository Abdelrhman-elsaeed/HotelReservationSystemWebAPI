
namespace Domain.Entities
{
    public class Feedback : BaseEntity
    {
        public string CustomerComment { get; set; }
        public int Rating { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public string StaffResponse { get; set; }
        public int? StaffId { get; set; }
        public Domain.Entities.User.User Staff { get; set; }
    }
}
