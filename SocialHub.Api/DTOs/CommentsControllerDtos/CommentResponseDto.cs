namespace SocialHub.Api.DTOs.CommentsControllerDtos
{
    public class CommentResponseDto
    {
        public required int CommentId { get; set; }
        public required string Text { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required int PostId { get; set; }
        public required Guid AuthorId { get; set; }
        public required string AuthorUserName { get; set; }
    }
}
