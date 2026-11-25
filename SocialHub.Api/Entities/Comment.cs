using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public required string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = [];
        public Guid AuthorId { get; set; }
        public ApplicationUser Author { get; set; } = null!;
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

    }
}
