using System.ComponentModel.DataAnnotations;

namespace BusinessAnalyticsSystem.Models
{
    // Reference Table 1: Product Categories
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

