using Microsoft.AspNetCore.Mvc;
using BT3_TH.Models;
using BT3_TH.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class TableLocationsController : Controller
{
    private readonly ITableLocationRepository _tableLocationRepository;

    public TableLocationsController(ITableLocationRepository tableLocationRepository)
    {
        _tableLocationRepository = tableLocationRepository;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var tableLocations = await _tableLocationRepository.GetAllAsync();
        return View(tableLocations);
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(TableLocation tableLocation)
    {
        if (ModelState.IsValid)
        {
            await _tableLocationRepository.AddAsync(tableLocation);
            return RedirectToAction(nameof(Index));
        }

        return View(tableLocation);
    }

    public async Task<IActionResult> Update(int id)
    {
        var tableLocation = await _tableLocationRepository.GetByIdAsync(id);
        if (tableLocation == null)
        {
            return NotFound();
        }

        return View(tableLocation);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, TableLocation tableLocation)
    {
        if (id != tableLocation.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _tableLocationRepository.UpdateAsync(tableLocation);
            return RedirectToAction(nameof(Index));
        }

        return View(tableLocation);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var tableLocation = await _tableLocationRepository.GetByIdAsync(id);
        if (tableLocation == null)
        {
            return NotFound();
        }

        return View(tableLocation);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _tableLocationRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}