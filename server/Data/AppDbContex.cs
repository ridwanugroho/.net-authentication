using Microsoft.EntityFrameworkCore;

namespace server.Model
{
    public class AppDbContex : DbContext
    {
        public DbSet<User> Users{get; set;}
        public DbSet<Activity> Activity{get; set;}

        public AppDbContex(DbContextOptions options) : base(options)
        {

        }
    }
}