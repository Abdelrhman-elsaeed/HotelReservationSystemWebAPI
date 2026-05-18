namespace Domain.Entities.Guest
{
    public class Guest : BaseEntity
    {
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string MobileNumber { get; set; }
        public ICollection<GuestReservationRoom> GuestReservationRooms { get; set; }

    }
}
