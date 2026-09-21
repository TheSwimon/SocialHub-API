using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public required string Text { get; set; }

        public string? Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = [];

    }
}
