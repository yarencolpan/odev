using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using odev.Entities;

namespace odev.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        public DbSet<User> Users { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);



            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId);
                
            // Seed Data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "adminpassword", Role = "Admin", CreatedAt = DateTime.UtcNow },
                new User { Id = 2, Username = "user1", Password = "userpassword", Role = "User", CreatedAt = DateTime.UtcNow }
            );
            
            modelBuilder.Entity<Flight>().HasData(
                new Flight { Id = 1, FlightNumber = "TK101", Origin = "Istanbul", Destination = "London", DepartureTime = DateTime.UtcNow.AddDays(1), ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(4), Price = 5000, Capacity = 150, CreatedAt = DateTime.UtcNow },
                new Flight { Id = 2, FlightNumber = "TK202", Origin = "Istanbul", Destination = "New York", DepartureTime = DateTime.UtcNow.AddDays(2), ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(10), Price = 15000, Capacity = 300, CreatedAt = DateTime.UtcNow }
            );
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }



        private void UpdateTimestamps()

        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
