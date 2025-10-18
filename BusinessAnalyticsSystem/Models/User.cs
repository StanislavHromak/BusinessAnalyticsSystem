using System.ComponentModel.DataAnnotations;

namespace BusinessAnalyticsSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Investor"; // Default role
    }
}
