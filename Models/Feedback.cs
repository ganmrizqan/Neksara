using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public enum FeedbackTargetType { Topic, Category }
        public FeedbackTargetType TargetType { get; set; }
        public enum FeedbackTargetId { TopicId, CategoryId }
        public FeedbackTargetId TargetId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;
        public bool IsVisible { get; set; } = true;

        public int UserId { get; set; }
        public User? User { get; set; }

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}