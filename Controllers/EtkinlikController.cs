using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Models;
using GonulluOlTarsus.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GonulluOlTarsus.Controllers;

public class EtkinlikController : Controller
{
    private readonly IEtkinlikService _etkinlikService;
    private readonly UserManager<Uye> _userManager;

    public EtkinlikController(IEtkinlikService etkinlikService, UserManager<Uye> userManager)
    {
        _etkinlikService = etkinlikService;
        _userManager = userManager;
    }

    // GET: /Etkinlik/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var etkinlik = await _etkinlikService.GetDetayAsync(id);
        if (etkinlik is null) return NotFound();

        var kullaniciId = _userManager.GetUserId(User);
        var model = new EtkinlikDetayViewModel
        {
            Id = etkinlik.Id,
            Baslik = etkinlik.Baslik,
            Aciklama = etkinlik.Aciklama,
            Tarih = etkinlik.Tarih,
            Konum = etkinlik.Konum,
            Kategori = etkinlik.Kategori,
            Kontenjan = etkinlik.Kontenjan,
            AdminOnaylandi = etkinlik.AdminOnaylandi,
            OlusturanAdSoyad = etkinlik.Uye?.TamAd ?? "Platform",
            OlusturulmaTarihi = etkinlik.OlusturulmaTarihi,
            Katilimcilar = etkinlik.Katilimlar.Select(k => new KatilimciViewModel
            {
                AdSoyad = k.Uye?.TamAd ?? "Anonim",
                KatilimTarihi = k.KatilimTarihi
            }),
            KullaniciGirisYapti = User.Identity?.IsAuthenticated ?? false,
            KullaniciZatenKatildi = kullaniciId != null && etkinlik.UyeZatenKatildi(kullaniciId),
            EtkinlikDolu = etkinlik.DoluMu(),
            EtkinlikGecmis = etkinlik.Tarih <= DateTime.UtcNow
        };

        return View(model);
    }

    // GET: /Etkinlik/Olustur
    [Authorize]
    public IActionResult Olustur() => View(new EtkinlikOlusturViewModel());

    // POST: /Etkinlik/Olustur
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(EtkinlikOlusturViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var uyeId = _userManager.GetUserId(User);
        if (uyeId is null) return Challenge();

        try
        {
            await _etkinlikService.OlusturAsync(
                model.Baslik,
                model.Aciklama,
                model.Tarih,
                model.Konum,
                model.Kategori,
                model.Kontenjan,
                uyeId);

            TempData["Mesaj"] = "Etkinliğiniz başarıyla oluşturuldu. Admin onayından sonra yayınlanacaktır.";
            TempData["MesajTipi"] = "success";
            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    // POST: /Etkinlik/Katil/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Katil(int id)
    {
        var uyeId = _userManager.GetUserId(User);
        if (uyeId is null) return Challenge();

        var basarili = await _etkinlikService.KatilAsync(id, uyeId);

        TempData["Mesaj"] = basarili
            ? "🎉 Etkinliğe başarıyla kaydoldunuz! Sizi aramızda görmek için sabırsızlanıyoruz."
            : "❌ Katılım gerçekleştirilemedi. Etkinlik dolu olabilir veya zaten kayıtlı olabilirsiniz.";
        TempData["MesajTipi"] = basarili ? "success" : "error";

        return RedirectToAction("Detay", new { id });
    }

    // POST: /Etkinlik/KatilimIptal/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KatilimIptal(int id)
    {
        var uyeId = _userManager.GetUserId(User);
        if (uyeId is null) return Challenge();

        var basarili = await _etkinlikService.KatilimIptalEtAsync(id, uyeId);

        TempData["Mesaj"] = basarili
            ? "Katılımınız iptal edildi."
            : "❌ İptal işlemi gerçekleştirilemedi.";
        TempData["MesajTipi"] = basarili ? "info" : "error";

        return RedirectToAction("Detay", new { id });
    }
}
