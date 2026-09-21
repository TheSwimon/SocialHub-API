using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.Entities
{
        public class Comment
        {
            public int CommentId { get; set; } 
            public required string Text { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public int AuthorId { get; set; }
            public User Author { get; set; } = null!;

            public int PostId { get; set; }
            public Post Post { get; set; } = null!;

        }
}
