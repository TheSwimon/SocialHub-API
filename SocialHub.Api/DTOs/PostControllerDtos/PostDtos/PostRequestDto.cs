using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.PostControllerDtos.PostDtos
{
    public class PostRequestDto
    {
        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public required string Text { get; set; }

        [MaxLength(300)]
        public string? Title { get; set; }
    }
}
