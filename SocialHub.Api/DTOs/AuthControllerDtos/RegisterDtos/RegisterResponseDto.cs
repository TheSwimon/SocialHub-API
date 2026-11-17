namespace SocialHub.Api.DTOs.AuthDtos.RegisterDtos
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
