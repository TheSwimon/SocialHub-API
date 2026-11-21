namespace SocialHub.Api.DTOs.PostsControllerDtos
{
    public class PostResponseDto
    {
        public int PostId { get; set; }
        public required string Text { get; set; }

        public string? Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required Guid AuthorId { get; set; }

        public required string AuthorUserName { get; set; }

    }
}
