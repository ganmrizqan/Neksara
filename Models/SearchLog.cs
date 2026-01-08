using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class SearchLog
    {
        [Key]
        public int SearchLogId { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public int ResultCount { get; set; } = 0;
        public DateTime SearchAt { get; set; } = DateTime.Now;
    }
}
