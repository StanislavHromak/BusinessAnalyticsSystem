using System.ComponentModel.DataAnnotations;

namespace BusinessAnalyticsSystem.Models
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Full name cannot exceed 500 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password (optional)")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,16}$",
            ErrorMessage = "Password must be 8–16 characters, include at least 1 uppercase letter, 1 digit, and 1 special character.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone number must be in the format +380XXXXXXXXX.")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
    }
}
