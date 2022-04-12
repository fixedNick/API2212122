using System.ComponentModel.DataAnnotations;

namespace Efimenko_API_Portfolio.Models.BodyModels
{
    public class ArticleUpdate
    {
        [Required]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Category { get; set; }
    }
}
