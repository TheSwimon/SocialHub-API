using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.LoginDtos;
using SocialHub.Api.DTOs.RegisterDtos;
using SocialHub.Api.Entities;
using SocialHub.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration, UserManager<ApplicationUser> userManager, IEmailService brevoEmailService, ILogger<AuthController> logger, IWebHostEnvironment _environment) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
        {

            var newUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Username
            };

            var result = await userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            { 
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem(ModelState);
            }

            if (_environment.IsDevelopment())
            {
                await userManager.ConfirmEmailAsync(newUser, await userManager.GenerateEmailConfirmationTokenAsync(newUser));   
            }
            else
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var confirmationLink = Url.Link("ConfirmEmail", new { userId = newUser.Id, token });

                try
                {
                    await brevoEmailService.SendAsync(newUser.Email, "Please confirm your email address", $"Please confirm your email address by clicking <a href=\"{confirmationLink}\">this link</a>.");

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send confirmation email to user {UserId}", newUser.Id);
                }
            }
            
           

            var response = new RegisterResponseDto()
            {
                UserId = newUser.Id,
                Email = newUser.Email,
                Username = newUser.UserName
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }



        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        { 

            var user = request.Login.Contains('@') ? await userManager.FindByEmailAsync(request.Login) : await userManager.FindByNameAsync(request.Login);
            const string invalidCredMessage = "Incorrect credentials.";

            if (user is null)
            {
                return Problem(title: invalidCredMessage, statusCode: StatusCodes.Status401Unauthorized);
            }

            if (await userManager.IsLockedOutAsync(user))
            {
                return Problem(title: invalidCredMessage, statusCode: StatusCodes.Status401Unauthorized);
            }

            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                await userManager.AccessFailedAsync(user);
                return Problem(title: invalidCredMessage, statusCode: StatusCodes.Status401Unauthorized);
            }

            if (!user.EmailConfirmed)
            {
                return Problem(title: "Please confirm your email first.", statusCode: StatusCodes.Status403Forbidden, extensions: new Dictionary<string, object?> { ["code"] = "EmailNotConfirmed" });
            }


            await userManager.ResetAccessFailedCountAsync(user);

            var token = GenerateJwtToken(user);

            return Ok(new LoginResponseDto { AccessToken = token });
        }

        [HttpGet("confirm-email", Name = "ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await userManager.FindByIdAsync(userId); 

            if (user is null)
            {
                return BadRequest(new { message = "Invalid confirmation link" });
            }

            var response = await userManager.ConfirmEmailAsync(user, token);

            if (!response.Succeeded)
            {
                return BadRequest(new { message = "Invalid confirmation link" });
            }

            return Ok(new { message = "Email confirmed, you can log in now." });
        }
        


        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                //Check
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)

            };

            var secretKey = configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured");
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);


            var tokenDescriptor = new JwtSecurityToken(
                issuer: "SocialHubApp",
                audience: "SocialHubApp",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),   
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
