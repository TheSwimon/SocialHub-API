using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.PostControllerDtos.CommentDtos;
using SocialHub.Api.Entities;
using System.Security.Claims;

namespace SocialHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(AppDbContext dbContext) : ControllerBase
    {
        [HttpPost("~/api/posts/{postId:int}/comments")]
        [Authorize]
        public async Task<ActionResult<CommentResponseDto>> CreateComment(int postId, CommentRequestDto request)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized("The authentication token is invalid or missing the user id claim.");
            }

            request.Text = request.Text.Trim();

            if (!await dbContext.Posts.AnyAsync(p => p.PostId == postId))
            {
                return NotFound("Post not found.");
            }

            var comment = new Comment
            {
                Text = request.Text,
                AuthorId = authorId,
                PostId = postId
            };

            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();


            // map the comment to the comment response dto

            var commentResponseDto = new CommentResponseDto
            {
                CommentId = comment.CommentId,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                PostId = comment.PostId,
                AuthorId = authorId,
                AuthorUserName = User.FindFirstValue(ClaimTypes.Name)!

            };

            return CreatedAtAction(nameof(GetComment), new { id = comment.CommentId }, commentResponseDto);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentResponseDto>> GetComment(int id)
        {
            var dto = await dbContext.Comments
                 .AsNoTracking()
                 .Where(c => c.CommentId == id)
                 .Select(c => new CommentResponseDto { CommentId = c.CommentId, Text = c.Text, CreatedAt = c.CreatedAt, PostId = c.PostId, AuthorId = c.AuthorId, AuthorUserName = c.Author.UserName! })
                 .FirstOrDefaultAsync();

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }
    }
}
