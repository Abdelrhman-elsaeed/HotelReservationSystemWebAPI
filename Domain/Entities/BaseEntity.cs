
namespace Domain.Entities
{
    public class BaseEntity
    {
        public int ID { get; set; }
        public bool Deleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
