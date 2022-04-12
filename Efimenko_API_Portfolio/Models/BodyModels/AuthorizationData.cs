using System.ComponentModel.DataAnnotations;

namespace Efimenko_API_Portfolio.Models.BodyModels
{
    public class AuthorizationData
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
