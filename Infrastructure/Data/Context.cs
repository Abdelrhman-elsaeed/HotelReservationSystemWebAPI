namespace Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) 
        { 
        }

        // Root Entities
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // ReservationManagement Entities
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationRoom> ReservationRooms { get; set; }

        // RoomManagement Entities
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomFacility> RoomFacilities { get; set; }
        public DbSet<RoomOffer> RoomOffers { get; set; }
        public DbSet<RoomPicture> RoomPictures { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        // UserManagement Entities
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .LogTo(log => Debug.WriteLine(log), LogLevel.Information)
                .EnableSensitiveDataLogging(); // have to disable it when it comes to production phase
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // disable Cascade Delete
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
