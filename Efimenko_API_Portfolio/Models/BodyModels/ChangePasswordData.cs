using System.ComponentModel.DataAnnotations;

namespace Efimenko_API_Portfolio.Models.BodyModels
{
    public class ChangePasswordData
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
