using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.RegisterDtos
{
    public class RegisterRequestDto
    {

        [MaxLength(30)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9]+(?:[_-][a-zA-Z0-9]+)*$")]
        [Required]
        public required string Username { get; set; }

        [MaxLength(254)]
        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [MinLength(8)]
        [MaxLength(128)]
        [Required]
        public required string Password { get; set; }
    }
}
