using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Entities;

namespace SocialHub.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ApplicationUser>(user =>
            {
                user.Property(u => u.UserName)
                .HasMaxLength(30);

                user.Property(u => u.Email)
                .HasMaxLength(254);

                user.Property(u => u.PasswordHash)
                .HasMaxLength(256);

                user.HasIndex(u => u.NormalizedEmail).IsUnique();

            });

            modelBuilder.Entity<Post>(post =>
            {
                post.Property(p => p.Title)
                .HasMaxLength(300);

                post.Property(p => p.Text)
                .HasMaxLength(4000);

            });

            modelBuilder.Entity<Comment>(comment =>
            {

                comment.Property(p => p.Text)
                .HasMaxLength(2000);

            });

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
