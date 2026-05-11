namespace Domain.Entities.RoomManagement
{
    public class RoomFacility
    {
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        [ForeignKey("Facility")]
        public int FacilityId { get; set; }
        public Room Room { get; set; }
        public Facility Facility { get; set; }
    }
}
