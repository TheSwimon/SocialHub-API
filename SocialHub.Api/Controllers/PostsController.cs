using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.PostsControllerDtos;
using SocialHub.Api.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


namespace SocialHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController(AppDbContext dbContext) : ControllerBase
    {
        

        [HttpPost]
        [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PostResponseDto>> CreatePost(PostRequestDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized();
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

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePost(int id, PostRequestDto request)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdClaim, out Guid authorId))
                {
                    return Unauthorized();
                }

                var post = await dbContext.Posts.FindAsync(id);

                if (post is null)
                {
                    return NotFound();
                }

                if (post.AuthorId != authorId)
                {
                    return Forbid();
                }

                post.Text = request.Text.Trim();
                post.Title = request.Title?.Trim();
                post.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return NoContent();
            }



        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
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

