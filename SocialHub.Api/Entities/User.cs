namespace SocialHub.Api.Entities
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public required string Username { get; set; } 
        public required string Email { get; set; } 
        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Post> Posts { get; set; } = new List<Post>();

    }
}
