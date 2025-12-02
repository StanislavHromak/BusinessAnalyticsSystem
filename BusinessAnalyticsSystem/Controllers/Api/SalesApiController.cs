using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Asp.Versioning;

namespace BusinessAnalyticsSystem.Controllers.Api
{
    [ApiController]
    [Route("api/v{version:apiVersion}/sales")]
    [ApiVersion("1.0")]
    public class SalesApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSales()
        {
            return await _context.Sales
                .Include(s => s.Product)
                .Include(s => s.Department)
                .OrderByDescending(s => s.SaleDateTime)
                .Select(s => new
                {
                    s.Id,
                    s.SaleDateTime,
                    s.Quantity,
                    s.UnitPrice,
                    s.TotalAmount,
                    s.CustomerName,
                    s.Notes,
                    ProductName = s.Product != null ? s.Product.Name : "N/A",
                    DepartmentName = s.Department != null ? s.Department.Name : "N/A",
                    s.ProductId,
                    s.DepartmentId
                })
                .ToListAsync();
        }

        // GET: api/v1/sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Product)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                sale.Id,
                sale.SaleDateTime,
                sale.Quantity,
                sale.UnitPrice,
                sale.TotalAmount,
                sale.CustomerName,
                sale.Notes,
                sale.ProductId,
                sale.DepartmentId,
                ProductName = sale.Product?.Name,
                DepartmentName = sale.Department?.Name
            });
        }

        // POST: api/v1/sales
        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale(Sale sale)
        {
            ModelState.Remove("Product");
            ModelState.Remove("Department");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(sale.ProductId);
            if (product == null)
            {
                return BadRequest("Invalid Product ID");
            }

            sale.UnitPrice = product.Price;
            sale.TotalAmount = sale.Quantity * sale.UnitPrice;
            sale.CreatedDate = DateTime.Now;

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id, version = "1.0" }, sale);
        }

        // PUT: api/v1/sales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, Sale sale)
        {
            if (id != sale.Id)
            {
                return BadRequest();
            }

            var existingSale = await _context.Sales.FindAsync(id);
            if (existingSale == null) return NotFound();

            existingSale.SaleDateTime = sale.SaleDateTime;
            existingSale.Quantity = sale.Quantity;
            existingSale.CustomerName = sale.CustomerName;
            existingSale.Notes = sale.Notes;
            existingSale.ProductId = sale.ProductId;
            existingSale.DepartmentId = sale.DepartmentId;

            var product = await _context.Products.FindAsync(sale.ProductId);
            if (product != null)
            {
                existingSale.UnitPrice = product.Price;
                existingSale.TotalAmount = existingSale.Quantity * existingSale.UnitPrice;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/v1/sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}