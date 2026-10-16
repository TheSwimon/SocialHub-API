using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.EmailConfirmationDtos
{
    public class EmailConfirmationResponseDto
    {
        [Required]
        public required string Message { get; set; }
        [Required]
        public required Guid UserId { get; set; }
    }
}
