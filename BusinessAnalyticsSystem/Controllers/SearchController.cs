using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;

namespace BusinessAnalyticsSystem.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Search
        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
            return View();
        }

        // POST: Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(SearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var query = _context.Sales
                .Include(s => s.Product)
                    .ThenInclude(p => p.Category)  // JOIN 1: Sale -> Product -> Category
                .Include(s => s.Department)        // JOIN 2: Sale -> Department
                .AsQueryable();

            // Search by date and time
            if (model.StartDate.HasValue)
            {
                query = query.Where(s => s.SaleDateTime >= model.StartDate.Value);
            }

            if (model.EndDate.HasValue)
            {
                query = query.Where(s => s.SaleDateTime <= model.EndDate.Value);
            }

            // Search by list of elements (categories)
            if (model.SelectedCategoryIds != null && model.SelectedCategoryIds.Any())
            {
                query = query.Where(s => model.SelectedCategoryIds.Contains(s.Product.CategoryId));
            }

            // Search by list of elements (departments)
            if (model.SelectedDepartmentIds != null && model.SelectedDepartmentIds.Any())
            {
                query = query.Where(s => model.SelectedDepartmentIds.Contains(s.DepartmentId));
            }

            // Search by value start (customer name)
            if (!string.IsNullOrWhiteSpace(model.CustomerNameStart))
            {
                query = query.Where(s => s.CustomerName != null && 
                    s.CustomerName.StartsWith(model.CustomerNameStart));
            }

            // Search by value end (customer name)
            if (!string.IsNullOrWhiteSpace(model.CustomerNameEnd))
            {
                query = query.Where(s => s.CustomerName != null && 
                    s.CustomerName.EndsWith(model.CustomerNameEnd));
            }

            // Search by value start (product name)
            if (!string.IsNullOrWhiteSpace(model.ProductNameStart))
            {
                query = query.Where(s => s.Product.Name.StartsWith(model.ProductNameStart));
            }

            // Search by value end (product name)
            if (!string.IsNullOrWhiteSpace(model.ProductNameEnd))
            {
                query = query.Where(s => s.Product.Name.EndsWith(model.ProductNameEnd));
            }

            var results = await query
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            model.Results = results;
            
            // Load lists for ViewBag
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
            
            return View("Index", model);
        }
    }

    public class SearchViewModel
    {
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Categories")]
        public List<int>? SelectedCategoryIds { get; set; }

        [Display(Name = "Departments")]
        public List<int>? SelectedDepartmentIds { get; set; }

        [Display(Name = "Customer Name (Start)")]
        public string? CustomerNameStart { get; set; }

        [Display(Name = "Customer Name (End)")]
        public string? CustomerNameEnd { get; set; }

        [Display(Name = "Product Name (Start)")]
        public string? ProductNameStart { get; set; }

        [Display(Name = "Product Name (End)")]
        public string? ProductNameEnd { get; set; }

        public List<Sale>? Results { get; set; }
    }
}

