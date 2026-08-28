namespace SocialHub.Api.Entities
{
    public class Post
    {
        public Guid PostId { get; set; } = Guid.NewGuid();

        public required string Content { get; set; }
        public string? Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
    }
}
