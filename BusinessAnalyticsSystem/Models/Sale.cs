using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessAnalyticsSystem.Models
{
    // Central table: Sales
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sale Date and Time")]
        public DateTime SaleDateTime { get; set; } = DateTime.Now;

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Unit Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Total Amount")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(200)]
        [Display(Name = "Customer")]
        public string? CustomerName { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Foreign key to Product
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Display(Name = "Product")]
        public Product? Product { get; set; }

        // Foreign key to Department reference table
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public Department? Department { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

