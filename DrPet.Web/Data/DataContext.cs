using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DrPet.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Animal> Animals { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
