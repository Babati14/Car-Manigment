using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Car_Manigment.Models;

namespace Car_Manigment.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<ServiceOrder> ServiceOrders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            modelBuilder.Entity<Car>()
                .HasMany(c => c.ServiceOrders)
                .WithOne(so => so.Car)
                .HasForeignKey(so => so.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Car.Owner (IdentityUser) relationship
            modelBuilder.Entity<Car>()
                .HasOne<IdentityUser>(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceOrder.CreatedBy (IdentityUser) relationship
            modelBuilder.Entity<ServiceOrder>()
                .HasOne<IdentityUser>(so => so.CreatedBy)
                .WithMany()
                .HasForeignKey(so => so.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}