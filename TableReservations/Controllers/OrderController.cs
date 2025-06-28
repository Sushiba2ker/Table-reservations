using TableReservations.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace TableReservations.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(o => o.OrderDetails).ToListAsync();

            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                Order = o,
                TotalPrice = o.OrderDetails.Sum(od => od.Quantity * od.Price)
            }).ToList();

            return View(orderViewModels);
        }
        
        public async Task<IActionResult> Revenue()
        {
            var orders = await _context.Orders.Include(o => o.OrderDetails).ToListAsync();

            var weeklyRevenue = orders
                .GroupBy(o => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.OrderDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .Select(g => new RevenueData { Period = $"Week {g.Key}", Revenue = g.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.Price)) })
                .ToList();

            var monthlyRevenue = orders
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new RevenueData { Period = $"Month {g.Key}", Revenue = g.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.Price)) })
                .ToList();

            var yearlyRevenue = orders
                .GroupBy(o => o.OrderDate.Year)
                .Select(g => new RevenueData { Period = $"Year {g.Key}", Revenue = g.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.Price)) })
                .ToList();

            var totalRevenue = orders.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.Price));

            var revenue = new Revenue
            {
                WeeklyRevenue = weeklyRevenue,
                MonthlyRevenue = monthlyRevenue,
                YearlyRevenue = yearlyRevenue,
                TotalRevenue = totalRevenue
            };

            return View(revenue);
        }

        

        // Thêm các action khác như Create, Edit, Delete...
    }
}