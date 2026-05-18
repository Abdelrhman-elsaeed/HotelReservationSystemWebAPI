using Domain.Entities.Guest;

namespace Domain.Entities.ReservationManagement
{
    public class ReservationRoom : BaseEntity
    {
        [ForeignKey("Reservation")]
        public int ReservationId { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Reservation Reservation { get; set; }
        public Room Room { get; set; }
        public ICollection<GuestReservationRoom> GuestReservationRooms { get; set; }

    }
}
