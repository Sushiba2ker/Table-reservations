using TableReservations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TableReservations.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Index
        public IActionResult Index()
        {
            return View();
        }
        

        // Add other actions as needed
    }
}