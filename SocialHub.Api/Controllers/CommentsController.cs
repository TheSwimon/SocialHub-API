using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs.CommentsControllerDtos;
using SocialHub.Api.Entities;
using System.Security.Claims;

namespace SocialHub.Api.Controllers
{
    [Route("api/posts/{postId:int}/comments")]
    [ApiController]
    public class CommentsController(AppDbContext dbContext) : ControllerBase
    {

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<ActionResult<CommentResponseDto>> CreateComment(int postId, CommentRequestDto request)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized();
            }

            if (!await dbContext.Posts.AnyAsync(p => p.PostId == postId))
            {
                return NotFound();
            }

            var text = request.Text.Trim();

            var comment = new Comment
            {
                Text = text,
                AuthorId = authorId,
                PostId = postId
            };

            if (request.ParentCommentId is not null)
            {
                var topComment = await dbContext.Comments.FindAsync(request.ParentCommentId);

                if (topComment is null || topComment.PostId != postId)
                {
                    return BadRequest();
                }

                comment.ParentCommentId = request.ParentCommentId;
            }


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

            return CreatedAtAction(nameof(GetComment), new { postId, commentId = comment.CommentId }, commentResponseDto);
        }


        [HttpPut("{commentId:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateComment(int postId, int commentId, CommentRequestDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid authorId)) {
                return Unauthorized();
            }

            var comment = await dbContext.Comments.FindAsync(commentId);

            if (comment is null || comment.PostId != postId)
            {
                return NotFound();
            }


            if (comment.AuthorId != authorId)
            {
                return Forbid();
            }

            comment.Text = request.Text.Trim();

            await dbContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{commentId:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteComment(int postId, int commentId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized();
            }

            var comment = await dbContext.Comments.FindAsync(commentId);

            if (comment is null || comment.PostId != postId)
            {
                return NotFound();
            }

            if (comment.AuthorId != authorId)
            {
                return Forbid();
            }

            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{commentId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CommentResponseDto>> GetComment(int postId, int commentId)
        {
            var dto = await dbContext.Comments
                 .AsNoTracking()
                 .Where(c => c.CommentId == commentId && c.PostId == postId)
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
