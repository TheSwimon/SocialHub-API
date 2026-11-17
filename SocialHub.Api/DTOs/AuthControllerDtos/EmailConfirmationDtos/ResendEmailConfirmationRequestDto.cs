using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.AuthDtos.EmailConfirmationDtos
{
    public class ResendEmailConfirmationRequestDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
