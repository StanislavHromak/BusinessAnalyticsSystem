using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Asp.Versioning;

namespace BusinessAnalyticsSystem.Controllers.Api
{
    [ApiController]
    [Route("api/v{version:apiVersion}/departments")]
    [ApiVersion("1.0")]
    public class DepartmentsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDepartments()
        {
            return await _context.Departments
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Manager,
                    d.Description,
                    d.CreatedDate,
                    SalesCount = d.Sales.Count
                })
                .ToListAsync();
        }

        // GET: api/v1/departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                department.Id,
                department.Name,
                department.Manager,
                department.Description,
                department.CreatedDate
            });
        }

        // POST: api/v1/departments
        [HttpPost]
        public async Task<ActionResult<Department>> CreateDepartment(Department department)
        {
            ModelState.Remove("Sales");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            department.CreatedDate = DateTime.Now;
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id, version = "1.0" }, department);
        }

        // PUT: api/v1/departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, Department department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            var existingDept = await _context.Departments.FindAsync(id);
            if (existingDept == null) return NotFound();

            existingDept.Name = department.Name;
            existingDept.Manager = department.Manager;
            existingDept.Description = department.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        // DELETE: api/v1/departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}