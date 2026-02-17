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

            modelBuilder.Entity<Car>()
                .HasMany(c => c.ServiceOrders)
                .WithOne(so => so.Car)
                .HasForeignKey(so => so.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Car>()
                .HasOne<IdentityUser>(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceOrder>()
                .HasOne<IdentityUser>(so => so.CreatedBy)
                .WithMany()
                .HasForeignKey(so => so.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}