using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Asp.Versioning;

namespace BusinessAnalyticsSystem.Controllers.Api
{
    [ApiController]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsApiController(AppDbContext context)
        {
            _context = context;
        }

        // === Версія 1.0 ===
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsV1()
        {
            return await _context.Products
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Code,
                    p.Price,
                    p.StockQuantity,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }

        // === Версія 2.0 (Нова логіка) ===
        [HttpGet]
        [ApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsV2()
        {
            return await _context.Products
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Code,
                    Price = p.Price,
                    Stock = p.StockQuantity,
                    Category = p.Category.Name,
                    // Нове поле в V2
                    InventoryValue = p.Price * p.StockQuantity,
                    LastUpdated = DateTime.Now
                })
                .ToListAsync();
        }

        // CRUD методи (загальні для обох версій або специфічні)
        [HttpPost]
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductsV1), new { id = product.Id, version = "1.0" }, product);
        }
    }
}
