namespace SocialHub.Api.DTOs.PostControllerDtos.PostDtos
{
    public class PostResponseDto
    {
        public int PostId { get; set; }
        public required string Text { get; set; }

        public string? Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
