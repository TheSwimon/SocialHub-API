using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.PostControllerDtos.CommentDtos;
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
                CreatedAt = post.CreatedAt
            };

            return CreatedAtAction(nameof(GetPost), new { id = postResponseDto.PostId }, postResponseDto); 
        }


        [HttpGet]
        public async Task<ActionResult<PostResponseDto>> GetPost([FromQuery] int id)
        {
            var dto = await dbContext.Posts
                .AsNoTracking()
                .Where(p => p.PostId == id)
                .Select(p => new PostResponseDto { PostId = p.PostId, Text = p.Text, CreatedAt = p.CreatedAt, Title = p.Title })
                .FirstOrDefaultAsync();

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpPost("{postId}/comments")]
        [Authorize]
        public async Task<ActionResult<CommentResponseDto>> CreateComment(int postId, [FromBody]CommentRequestDto commentRequestDto)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized("The authentication token is invalid or missing the user id claim.");
            }

            if (string.IsNullOrWhiteSpace(commentRequestDto.Text))
            {
                return BadRequest("Cannot create a comment with empty text.");
            }

            var post = await dbContext.Posts.FindAsync(postId);

            if (post is null)
            {
                return NotFound("Post not found.");
            }

            var comment = new Comment
            {
                Text = commentRequestDto.Text,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow,
                PostId = postId
            };

            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();


            // map the comment to the comment response dto

            var commentResponseDto = new CommentResponseDto
            {
                CommentId = comment.CommentId,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };

            return CreatedAtAction(nameof(GetComment), new { id = comment.CommentId }, commentResponseDto);
        }


        [HttpGet]
        public async Task<ActionResult<CommentResponseDto>> GetComment([FromQuery] int id)
        {
            var dto = await dbContext.Comments
                 .AsNoTracking()
                 .Where(c => c.CommentId == id)
                 .Select(c => new CommentResponseDto { CommentId = c.CommentId, Text = c.Text, CreatedAt = c.CreatedAt})
                 .FirstOrDefaultAsync();

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

    }
}   

