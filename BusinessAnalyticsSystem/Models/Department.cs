using System.ComponentModel.DataAnnotations;

namespace BusinessAnalyticsSystem.Models
{
    // Reference Table 2: Departments
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Manager")]
        public string? Manager { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}

