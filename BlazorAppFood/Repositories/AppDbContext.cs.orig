using Microsoft.EntityFrameworkCore;
using BlazorAppFood.Models;

namespace BlazorAppFood.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
    }
}