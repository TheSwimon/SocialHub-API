using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.RegisterDtos;
using SocialHub.Api.Entities;

namespace SocialHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AppDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        [Route("/register")]
        public async Task<ActionResult<RegisteruserResponseDto>> Register(RegisterUserRequestDto request)
        {
            string normalizedEmail = request.Email.Trim();
            string normalizedUsername = request.Username.Trim();

            if (await dbContext.Users.AnyAsync(u => u.Email == normalizedEmail))
            {
                return Conflict(new { message = "Email already exists" });
            }
            else if (await dbContext.Users.AnyAsync(u => u.Username == normalizedUsername))
            {
                return Conflict(new { message = "Username already exists" });
            }   
            

            var newUser = new User
            {
                Email = normalizedEmail,
                Username = normalizedUsername,
                PasswordHash = ""
            };

            var passwordHash = new PasswordHasher<User>().HashPassword(newUser, request.Password);
            newUser.PasswordHash = passwordHash;



            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();



            var responseDto = new RegisteruserResponseDto()
            {
                Email = newUser.Email,
                Username = newUser.Username
            };

            return Ok(responseDto);
        }
    }
}
