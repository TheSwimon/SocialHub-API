using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.CommentsControllerDtos
{
    public class CommentRequestDto
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public required string Text { get; set; }

        public int? ParentCommentId { get; set; }
    }
}
