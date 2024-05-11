using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BT3_TH.Models;
using BT3_TH.Repositories;

namespace BT3_TH.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductRepository _productRepository;

    public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllAsync();
        return View(products);
    }

    public IActionResult About()
    {
        return View();
    }
    public async Task<IActionResult> Menu()
    {
        var products = await _productRepository.GetAllAsync();
        return View(products);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public async Task<IActionResult> Menubutton()
    {
        var products = await _productRepository.GetAllAsync();
        return View(products);
    }
}