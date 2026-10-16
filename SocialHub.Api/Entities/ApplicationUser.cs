
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Post> Posts { get; set; } = [];

        public ICollection<Comment> Comments { get; set; } = [];

    }   
}
