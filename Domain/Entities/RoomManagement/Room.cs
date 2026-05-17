namespace Domain.Entities.RoomManagement
{
    public class Room : BaseEntity
    {
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }
        public ICollection<RoomPicture> Pictures { get; set; }
        public ICollection<RoomFacility> RoomFacilities { get; set; }
        public ICollection<RoomOffer> RoomOffers { get; set; }
    }
}
