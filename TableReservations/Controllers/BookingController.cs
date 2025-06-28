using Microsoft.AspNetCore.Mvc;
using TableReservations.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TableReservations.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Booking()
        {
            var tableLocations = await _context.TableLocations.ToListAsync();
            ViewBag.TableLocations = new SelectList(tableLocations, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            ModelState.Remove("SpecialRequest"); // Loại bỏ xác thực ModelState cho SpecialRequest

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menubutton", "Home"); // Chuyển hướng đến trang Home/Menu sau khi đặt bàn
            }
            return View(booking);
        }
        
        
    }
}