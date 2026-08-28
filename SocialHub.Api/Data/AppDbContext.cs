using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Entities;

namespace SocialHub.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
