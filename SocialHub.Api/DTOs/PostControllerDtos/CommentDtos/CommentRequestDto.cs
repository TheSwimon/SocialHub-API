using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.PostControllerDtos.CommentDtos
{
    public class CommentRequestDto
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public required string Text { get; set; }
    }
}
