using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;

namespace BusinessAnalyticsSystem.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly AppDbContext _context;

        public SaleController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Product).ThenInclude(p => p.Category)
                .Include(s => s.Department)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();
            return View(sales);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var sale = await _context.Sales
                .Include(s => s.Product).ThenInclude(p => p.Category)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null) return NotFound();
            return View(sale);
        }

        // ==== CREATE ====
        [Authorize(Roles = "Admin,Owner,Manager")] 
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner,Manager")]
        public async Task<IActionResult> Create([Bind("Id,SaleDateTime,Quantity,CustomerName,Notes,ProductId,DepartmentId")] Sale sale)
        {
            var product = await _context.Products.FindAsync(sale.ProductId);

            if (product != null)
            {
                sale.UnitPrice = product.Price; 
                sale.TotalAmount = sale.UnitPrice * sale.Quantity; 
                sale.CreatedDate = DateTime.Now;

                ModelState.Remove("Product");
                ModelState.Remove("Department");
            }

            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", sale.ProductId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", sale.DepartmentId);
            return View(sale);
        }

        // ==== EDIT ====
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", sale.ProductId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", sale.DepartmentId);
            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SaleDateTime,Quantity,UnitPrice,TotalAmount,CustomerName,Notes,ProductId,DepartmentId,CreatedDate")] Sale sale)
        {
            if (id != sale.Id) return NotFound();

            if (ModelState.IsValid)
            {
                sale.TotalAmount = sale.UnitPrice * sale.Quantity;

                _context.Update(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", sale.ProductId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", sale.DepartmentId);
            return View(sale);
        }

        // ==== DELETE ====
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var sale = await _context.Sales
                .Include(s => s.Product)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null) return NotFound();
            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
