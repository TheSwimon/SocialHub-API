
using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Post> Posts { get; set; } = [];

        public ICollection<Comment> Comments { get; set; } = [];

    }   
}
