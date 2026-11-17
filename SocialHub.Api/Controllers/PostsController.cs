using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.PostControllerDtos.PostDtos;
using SocialHub.Api.Entities;
using System.Security.Claims;

namespace SocialHub.Api.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class PostsController(AppDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreatePost(PostRequestDto request)
        {
            request.Text = request.Text.Trim();
            request.Title = request.Title?.Trim();

            dbContext.Posts.Add(new Post { Title = request.Title, Text = request.Text, AuthorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)});

            await dbContext.SaveChangesAsync();

            return Ok();
        } 
    }
}
