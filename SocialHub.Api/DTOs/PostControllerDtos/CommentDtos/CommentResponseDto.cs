namespace SocialHub.Api.DTOs.PostControllerDtos.CommentDtos
{
    public class CommentResponseDto
    {
        public required int CommentId { get; set; }
        public required string Text { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
