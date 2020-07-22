using DrPet.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrPet.Web.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
