using GonulluOlTarsus.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GonulluOlTarsus.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IEtkinlikService _etkinlikService;

    public AdminController(IEtkinlikService etkinlikService)
    {
        _etkinlikService = etkinlikService;
    }

    // GET: /Admin
    public async Task<IActionResult> Index()
    {
        var onayBekleyenler = await _etkinlikService.GetOnayBekleyenlerAsync();
        return View(onayBekleyenler);
    }

    // POST: /Admin/Onayla/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Onayla(int id)
    {
        await _etkinlikService.AdminOnaylaAsync(id);
        TempData["Mesaj"] = "✅ Etkinlik onaylandı ve yayınlandı.";
        TempData["MesajTipi"] = "success";
        return RedirectToAction("Index");
    }

    // POST: /Admin/Reddet/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reddet(int id)
    {
        await _etkinlikService.AdminOnayiGeriAlAsync(id);
        TempData["Mesaj"] = "Etkinlik reddedildi.";
        TempData["MesajTipi"] = "info";
        return RedirectToAction("Index");
    }
}
