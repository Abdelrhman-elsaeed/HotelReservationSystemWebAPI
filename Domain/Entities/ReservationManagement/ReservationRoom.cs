
namespace Domain.Entities.ReservationManagement
{
    public class ReservationRoom : BaseEntity
    {
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public ICollection<Guest> RoomGuests { get; set; }
    }
}
