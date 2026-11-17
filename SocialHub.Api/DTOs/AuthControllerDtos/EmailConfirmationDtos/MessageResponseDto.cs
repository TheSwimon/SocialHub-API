using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.AuthDtos.EmailConfirmationDtos
{
    public class MessageResponseDto
    {
        [Required]
        public required string Message { get; set; }
    }
}
