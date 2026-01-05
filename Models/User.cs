using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<TopicView>? TopicViews { get; set; } = new HashSet<TopicView>();
        public ICollection<Feedback>? Feedbacks { get; set; } = new HashSet<Feedback>();
    }
}
