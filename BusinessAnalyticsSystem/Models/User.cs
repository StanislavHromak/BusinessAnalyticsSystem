using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BusinessAnalyticsSystem.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        [StringLength(500, ErrorMessage = "Full name cannot exceed 500 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone number must be in the format +380XXXXXXXXX.")]
        public override string PhoneNumber 
        { 
            get => base.PhoneNumber; 
            set => base.PhoneNumber = value; 
        }
    }
}

