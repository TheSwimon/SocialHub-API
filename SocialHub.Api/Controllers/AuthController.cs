using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.RegisterDtos;
using SocialHub.Api.Entities;

namespace SocialHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AppDbContext dbContext) : ControllerBase
    {
        
    }
}
