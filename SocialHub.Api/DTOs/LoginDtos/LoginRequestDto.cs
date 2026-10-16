using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs.LoginDtos
{
    public class LoginRequestDto
    {
        [Required, StringLength(254)]
        public required string Login { get; set; }

        [Required, StringLength(128)]
        public required string Password { get; set; }
    }
}
