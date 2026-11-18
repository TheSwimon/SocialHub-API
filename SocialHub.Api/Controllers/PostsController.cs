using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<PostResponseDto>> CreatePost(PostRequestDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized("The authentication token is invalid or missing the user id claim.");
            }


            request.Text = request.Text.Trim();
            request.Title = request.Title?.Trim();

            var post = new Post { Title = request.Title, Text = request.Text, AuthorId = authorId };

            dbContext.Posts.Add(post);

            await dbContext.SaveChangesAsync();

            // map entity to response dto
            var postResponseDto = new PostResponseDto
            {
                PostId = post.PostId,
                Text = post.Text,
                Title = post.Title,
                CreatedAt = post.CreatedAt,
                AuthorId = authorId,
                AuthorUserName = User.FindFirstValue(ClaimTypes.Name)!
            };

            return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, postResponseDto); 
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<PostResponseDto>> GetPost(int id)
        {
            var dto = await dbContext.Posts
                .AsNoTracking()
                .Where(p => p.PostId == id)
                .Select(p => new PostResponseDto { PostId = p.PostId, Title = p.Title, Text = p.Text, CreatedAt = p.CreatedAt, AuthorId = p.AuthorId, AuthorUserName = p.Author.UserName!})
                .FirstOrDefaultAsync();

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

    }
}   

