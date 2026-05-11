namespace Domain.Entities.RoomManagement
{
    public class RoomPicture : BaseEntity
    {
        public string PictureUrl { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
