using BusinessAnalyticsSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessAnalyticsSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Delete existing data to ensure fresh English data
            context.Sales.RemoveRange(context.Sales);
            context.Products.RemoveRange(context.Products);
            context.Departments.RemoveRange(context.Departments);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();

            // Create Reference Table 1: Categories
            var categories = new Category[]
            {
                new Category { Name = "Electronics", Description = "Electronic devices and accessories", CreatedDate = DateTime.Now.AddDays(-30) },
                new Category { Name = "Clothing", Description = "Clothing and footwear", CreatedDate = DateTime.Now.AddDays(-25) },
                new Category { Name = "Food Products", Description = "Food items", CreatedDate = DateTime.Now.AddDays(-20) },
                new Category { Name = "Furniture", Description = "Home and office furniture", CreatedDate = DateTime.Now.AddDays(-15) },
                new Category { Name = "Books", Description = "Books and educational materials", CreatedDate = DateTime.Now.AddDays(-10) }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Create Reference Table 2: Departments
            var departments = new Department[]
            {
                new Department { Name = "Sales Department", Manager = "John Smith", Description = "Responsible for product sales", CreatedDate = DateTime.Now.AddDays(-30) },
                new Department { Name = "Marketing Department", Manager = "Mary Johnson", Description = "Marketing campaigns and advertising", CreatedDate = DateTime.Now.AddDays(-28) },
                new Department { Name = "Logistics Department", Manager = "Alex Brown", Description = "Delivery and warehouse operations", CreatedDate = DateTime.Now.AddDays(-25) },
                new Department { Name = "Customer Service", Manager = "Emma Wilson", Description = "Customer support and service", CreatedDate = DateTime.Now.AddDays(-22) },
                new Department { Name = "Online Department", Manager = "David Lee", Description = "Online sales and e-commerce", CreatedDate = DateTime.Now.AddDays(-18) }
            };

            context.Departments.AddRange(departments);
            context.SaveChanges();

            // Create Products
            var products = new Product[]
            {
                new Product { Name = "Samsung Smartphone", Code = "SM-001", Price = 15000, StockQuantity = 50, CategoryId = categories[0].Id, CreatedDate = DateTime.Now.AddDays(-20) },
                new Product { Name = "Dell Laptop", Code = "DL-002", Price = 35000, StockQuantity = 25, CategoryId = categories[0].Id, CreatedDate = DateTime.Now.AddDays(-18) },
                new Product { Name = "Men's T-Shirt", Code = "TS-003", Price = 500, StockQuantity = 200, CategoryId = categories[1].Id, CreatedDate = DateTime.Now.AddDays(-15) },
                new Product { Name = "Jeans", Code = "JN-004", Price = 1200, StockQuantity = 150, CategoryId = categories[1].Id, CreatedDate = DateTime.Now.AddDays(-12) },
                new Product { Name = "White Bread", Code = "BR-005", Price = 25, StockQuantity = 500, CategoryId = categories[2].Id, CreatedDate = DateTime.Now.AddDays(-10) },
                new Product { Name = "Milk", Code = "ML-006", Price = 35, StockQuantity = 300, CategoryId = categories[2].Id, CreatedDate = DateTime.Now.AddDays(-8) },
                new Product { Name = "Office Desk", Code = "TB-007", Price = 4500, StockQuantity = 40, CategoryId = categories[3].Id, CreatedDate = DateTime.Now.AddDays(-5) },
                new Product { Name = "Office Chair", Code = "CH-008", Price = 3200, StockQuantity = 60, CategoryId = categories[3].Id, CreatedDate = DateTime.Now.AddDays(-3) },
                new Product { Name = "C# Programming Book", Code = "BK-009", Price = 450, StockQuantity = 100, CategoryId = categories[4].Id, CreatedDate = DateTime.Now.AddDays(-2) },
                new Product { Name = "Database Fundamentals Book", Code = "BK-010", Price = 380, StockQuantity = 80, CategoryId = categories[4].Id, CreatedDate = DateTime.Now.AddDays(-1) }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // Create Sales (Central table)
            var random = new Random();
            var sales = new List<Sale>();

            for (int i = 0; i < 50; i++)
            {
                var product = products[random.Next(products.Length)];
                var department = departments[random.Next(departments.Length)];
                var quantity = random.Next(1, 10);
                var saleDate = DateTime.Now.AddDays(-random.Next(0, 30)).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));

                sales.Add(new Sale
                {
                    SaleDateTime = saleDate,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    TotalAmount = product.Price * quantity,
                    CustomerName = $"Customer {i + 1}",
                    Notes = $"Sale #{i + 1}",
                    ProductId = product.Id,
                    DepartmentId = department.Id,
                    CreatedDate = saleDate
                });
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();
        }
    }
}

