using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DrPet.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Animal> Animals { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>()
                .Property(a => a.DateOfBirth)
                .HasColumnType("date");

            modelBuilder.Entity<User>()
              .Property(a => a.DateOfBirth)
              .HasColumnType("date");

            modelBuilder.Entity<Appointment>()
             .Property(a => a.StartTime)
             .HasColumnType("datetime");

            //Cascading Delete Rule
            var cascadesFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadesFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
