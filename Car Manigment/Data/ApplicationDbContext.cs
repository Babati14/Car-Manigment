using Microsoft.EntityFrameworkCore;
using Car_Manigment.Models;

namespace Car_Manigment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints if needed (kept simple for now)
            modelBuilder.Entity<Car>()
                .HasMany(c => c.ServiceOrders)
                .WithOne(so => so.Car)
                .HasForeignKey(so => so.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}