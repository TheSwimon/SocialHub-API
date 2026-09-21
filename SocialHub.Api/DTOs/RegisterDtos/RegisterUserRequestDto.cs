using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.RegisterDtos
{
    public class RegisterUserRequestDto
    {

        [MaxLength(30)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9](?:[a-zA-Z0-9]*[_-]?[a-zA-Z0-9]+)*$")]
        public required string Username { get; set; }

        [MaxLength(254)]
        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6)]
        [MaxLength(50)]
        public required string Password { get; set; }
    }
}
