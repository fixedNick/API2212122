using System.ComponentModel.DataAnnotations;

namespace Efimenko_API_Portfolio.Models.BodyModels
{
    public class PersonUpdate
    {
        [Required]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Country { get; set; }
        public bool? IsAdmin { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? Achievments { get; set; }
        public string? Articles { get; set; }

    }
}
