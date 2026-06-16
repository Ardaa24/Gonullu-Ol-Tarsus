using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Models;
using GonulluOlTarsus.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GonulluOlTarsus.Controllers;

[Authorize(Roles = "Admin,Super Admin")]
public class AdminController : Controller
{
    private readonly IEtkinlikService _etkinlikService;
    private readonly UserManager<Uye> _userManager;

    public AdminController(IEtkinlikService etkinlikService, UserManager<Uye> userManager)
    {
        _etkinlikService = etkinlikService;
        _userManager = userManager;
    }

    // GET: /Admin (Dashboard)
    public async Task<IActionResult> Index()
    {
        var topUye = await _userManager.Users.CountAsync();
        var istatistikler = await _etkinlikService.GetDashboardIstatistikleriAsync();
        var tumEtkinlikler = (await _etkinlikService.GetTumEtkinliklerAdminAsync()).ToList();

        var model = new AdminDashboardViewModel
        {
            ToplamUyeSayisi = topUye,
            ToplamEtkinlikSayisi = istatistikler.ToplamAktifEtkinlik,
            OnayBekleyenEtkinlikSayisi = istatistikler.OnayBekleyenSayisi,
            ToplamKatilimSayisi = istatistikler.ToplamKatilim,
            
            // Son 7 Gün grafikleri için sahte veriler yerine gerçeğe yakın veriler
            Son7GunEtiketleri = Enumerable.Range(0, 7).Select(i => DateTime.UtcNow.AddDays(-6 + i).ToString("dd MMM")).ToList(),
            Son7GunUyeKayitlari = Enumerable.Range(0, 7).Select(i => _userManager.Users.Count(u => u.KayitTarihi.Date == DateTime.UtcNow.AddDays(-6 + i).Date)).ToList(),
            Son7GunEtkinlikleri = Enumerable.Range(0, 7).Select(i => tumEtkinlikler.Count(e => e.OlusturulmaTarihi.Date == DateTime.UtcNow.AddDays(-6 + i).Date)).ToList()
        };

        return View(model);
    }

    // GET: /Admin/Uyeler
    public async Task<IActionResult> Kullanicilar()
    {
        var users = await _userManager.Users.ToListAsync();
        var model = new List<AdminKullaniciViewModel>();
        
        foreach(var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            model.Add(new AdminKullaniciViewModel
            {
                Id = user.Id,
                TamAd = user.TamAd,
                Email = user.Email ?? "",
                Rol = roles.FirstOrDefault() ?? "Gönüllü"
            });
        }

        return View(model);
    }

    // GET: /Admin/KullaniciDuzenle/5
    public async Task<IActionResult> KullaniciDuzenle(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Admin kendi rol arkadaşlarını veya Super Adminleri düzenleyemez (Super Admin herkesi düzenleyebilir)
        var (yetkiVar, hataMesaji) = await YetkiKontroluYapAsync(user);
        if (!yetkiVar)
        {
            TempData["Mesaj"] = hataMesaji;
            TempData["MesajTipi"] = "error";
            return RedirectToAction(nameof(Kullanicilar));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var model = new AdminKullaniciDuzenleViewModel
        {
            Id = user.Id,
            TamAd = user.TamAd,
            Email = user.Email ?? "",
            Rol = roles.FirstOrDefault() ?? "Gonullu"
        };

        return View(model);
    }

    // POST: /Admin/KullaniciDuzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KullaniciDuzenle(AdminKullaniciDuzenleViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        // Yetki Kontrolü
        var (yetkiVar, hataMesaji) = await YetkiKontroluYapAsync(user);
        if (!yetkiVar)
        {
            TempData["Mesaj"] = hataMesaji;
            TempData["MesajTipi"] = "error";
            return RedirectToAction(nameof(Kullanicilar));
        }

        var mevcutKullanici = await _userManager.GetUserAsync(User);
        var mevcutRoller = await _userManager.GetRolesAsync(mevcutKullanici!);
        // Admin başka birini Super Admin yapamaz
        if (!mevcutRoller.Contains("Super Admin") && model.Rol == "Super Admin")
        {
            ModelState.AddModelError("", "Süper Admin yetkisi verme hakkınız yok.");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.TamAd))
        {
            ModelState.AddModelError("TamAd", "Ad Soyad boş olamaz.");
            return View(model);
        }

        var adSoyad = model.TamAd.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        user.Ad = adSoyad.Length > 0 ? adSoyad[0] : "İsimsiz";
        user.Soyad = adSoyad.Length > 1 ? adSoyad[1] : "";
        
        user.Email = model.Email;
        user.UserName = model.Email;

        await _userManager.UpdateAsync(user);

        // Şifre değiştirme
        if (!string.IsNullOrWhiteSpace(model.YeniSifre))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.YeniSifre);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("YeniSifre", err.Description);
                return View(model);
            }
        }

        // Rol değiştirme
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(model.Rol))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.Rol);
        }

        TempData["Mesaj"] = "Kullanıcı başarıyla güncellendi.";
        TempData["MesajTipi"] = "success";
        return RedirectToAction(nameof(Kullanicilar));
    }

    // GET: /Admin/Etkinlikler
    public async Task<IActionResult> Etkinlikler()
    {
        var etkinlikler = await _etkinlikService.GetTumEtkinliklerAdminAsync();
        
        var model = etkinlikler.Select(e => new AdminEtkinlikListeViewModel
        {
            Id = e.Id,
            Baslik = e.Baslik,
            Olusturan = e.Uye?.TamAd ?? "Bilinmiyor",
            Tarih = e.Tarih,
            KatilimciSayisi = e.MevcutKatilimciSayisi(),
            Kontenjan = e.Kontenjan,
            AdminOnaylandi = e.AdminOnaylandi,
            IptalEdildi = e.IptalEdildi
        }).ToList();

        return View(model);
    }

    // GET: /Admin/EtkinlikDuzenle/5
    public async Task<IActionResult> EtkinlikDuzenle(int id)
    {
        var etkinlik = await _etkinlikService.GetDetayAsync(id);
        if (etkinlik == null) return NotFound();

        var model = new AdminEtkinlikDuzenleViewModel
        {
            Id = etkinlik.Id,
            Baslik = etkinlik.Baslik,
            Aciklama = etkinlik.Aciklama,
            Tarih = etkinlik.Tarih,
            Konum = etkinlik.Konum,
            Kategori = etkinlik.Kategori,
            Kontenjan = etkinlik.Kontenjan
        };

        return View(model);
    }

    // POST: /Admin/EtkinlikDuzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EtkinlikDuzenle(AdminEtkinlikDuzenleViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var basarili = await _etkinlikService.AdminEtkinlikGuncelleAsync(
            model.Id, model.Baslik, model.Aciklama, model.Tarih, model.Konum, model.Kategori, model.Kontenjan);

        if (basarili)
        {
            TempData["Mesaj"] = "Etkinlik başarıyla güncellendi.";
            TempData["MesajTipi"] = "success";
            return RedirectToAction(nameof(Etkinlikler));
        }

        ModelState.AddModelError("", "Etkinlik güncellenirken bir hata oluştu (Kontenjan katılımcı sayısından az olamaz vs).");
        return View(model);
    }

    // POST: /Admin/EtkinlikSil/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EtkinlikSil(int id)
    {
        await _etkinlikService.AdminEtkinlikSilAsync(id);
        TempData["Mesaj"] = "Etkinlik başarıyla silindi.";
        TempData["MesajTipi"] = "success";
        return RedirectToAction(nameof(Etkinlikler));
    }

    // POST: /Admin/Onayla/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Onayla(int id)
    {
        await _etkinlikService.AdminOnaylaAsync(id);
        TempData["Mesaj"] = "✅ Etkinlik onaylandı ve yayınlandı.";
        TempData["MesajTipi"] = "success";
        return RedirectToAction(nameof(Etkinlikler));
    }

    // POST: /Admin/Reddet/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reddet(int id)
    {
        await _etkinlikService.AdminOnayiGeriAlAsync(id);
        TempData["Mesaj"] = "Etkinlik reddedildi.";
        TempData["MesajTipi"] = "info";
        return RedirectToAction(nameof(Etkinlikler));
    }

    /// <summary>
    /// Giriş yapan kullanıcının hedef kullanıcıyı düzenleme yetkisini kontrol eder.
    /// </summary>
    private async Task<(bool YetkiVar, string? HataMesaji)> YetkiKontroluYapAsync(Uye hedefKullanici)
    {
        var hedefRoller = await _userManager.GetRolesAsync(hedefKullanici);
        var mevcutKullanici = await _userManager.GetUserAsync(User);
        if (mevcutKullanici == null) return (false, "Oturum bilgisi alınamadı.");
        
        var mevcutRoller = await _userManager.GetRolesAsync(mevcutKullanici);

        if (!mevcutRoller.Contains("Super Admin"))
        {
            if (hedefRoller.Contains("Super Admin") || (hedefRoller.Contains("Admin") && hedefKullanici.Id != mevcutKullanici.Id))
            {
                return (false, "Yetkiniz bu kullanıcıyı düzenlemek için yeterli değil.");
            }
        }

        return (true, null);
    }
}
