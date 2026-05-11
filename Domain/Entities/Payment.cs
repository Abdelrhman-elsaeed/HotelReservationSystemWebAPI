
namespace Domain.Entities
{
    public class Payment : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
