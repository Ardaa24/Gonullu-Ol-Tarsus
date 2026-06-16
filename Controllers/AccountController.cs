using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Models;
using GonulluOlTarsus.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GonulluOlTarsus.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<Uye> _userManager;
    private readonly SignInManager<Uye> _signInManager;
    private readonly IEtkinlikService _etkinlikService;

    public AccountController(UserManager<Uye> userManager, SignInManager<Uye> signInManager, IEtkinlikService etkinlikService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _etkinlikService = etkinlikService;
    }

    // GET: /Account/Kayit
    public IActionResult Kayit(string? geriDonusUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["GeriDonusUrl"] = geriDonusUrl;
        return View();
    }

    // POST: /Account/Kayit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kayit(KayitViewModel model, string? geriDonusUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var uye = new Uye
        {
            UserName = model.Email,
            Email = model.Email,
            Ad = model.Ad,
            Soyad = model.Soyad,
            EmailConfirmed = true, // Geliştirme ortamında onaysız açık
            KayitTarihi = DateTime.UtcNow
        };

        var sonuc = await _userManager.CreateAsync(uye, model.Sifre);

        if (sonuc.Succeeded)
        {
            await _userManager.AddToRoleAsync(uye, "Gonullu");
            await _signInManager.SignInAsync(uye, isPersistent: false);

            TempData["Mesaj"] = $"🎉 Hoş geldiniz, {uye.Ad}! Artık Tarsus gönüllü topluluğunun bir parçasısınız.";
            TempData["MesajTipi"] = "success";

            return !string.IsNullOrEmpty(geriDonusUrl) && Url.IsLocalUrl(geriDonusUrl)
                ? Redirect(geriDonusUrl)
                : RedirectToAction("Index", "Home");
        }

        foreach (var hata in sonuc.Errors)
            ModelState.AddModelError(string.Empty, HataMesajiniCevir(hata.Code));

        return View(model);
    }

    // GET: /Account/Giris
    public IActionResult Giris(string? geriDonusUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new GirisViewModel { GeriDonusUrl = geriDonusUrl });
    }

    // POST: /Account/Giris
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Giris(GirisViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var sonuc = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Sifre,
            model.BeniHatirla,
            lockoutOnFailure: true);

        if (sonuc.Succeeded)
        {
            return !string.IsNullOrEmpty(model.GeriDonusUrl) && Url.IsLocalUrl(model.GeriDonusUrl)
                ? Redirect(model.GeriDonusUrl)
                : RedirectToAction("Index", "Home");
        }

        if (sonuc.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Hesabınız geçici olarak kilitlendi. Lütfen birkaç dakika sonra tekrar deneyin.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "E-posta adresi veya şifre hatalı.");
        return View(model);
    }

    // GET: /Account/Profil/{id?}
    [Authorize]
    public async Task<IActionResult> Profil(string? id)
    {
        var mevcutKullaniciId = _userManager.GetUserId(User);
        var hedefId = id ?? mevcutKullaniciId;
        if (hedefId is null) return Challenge();

        var uye = await _userManager.FindByIdAsync(hedefId);
        if (uye is null) return NotFound();

        var roller = await _userManager.GetRolesAsync(uye);
        var katildigiEtkinlikler = await _etkinlikService.GetUyeKatildigiEtkinliklerAsync(hedefId);
        var olusturduguEtkinlikler = await _etkinlikService.GetUyeEtkinlikleriAsync(hedefId);

        var model = new ProfilViewModel
        {
            Id = uye.Id,
            TamAd = uye.TamAd,
            Email = uye.Email ?? "",
            Biyografi = uye.Biyografi,
            KayitTarihi = uye.KayitTarihi,
            Rol = roller.FirstOrDefault() ?? "Gönüllü",
            KendiProfili = hedefId == mevcutKullaniciId,
            KatildigiEtkinlikler = katildigiEtkinlikler.Select(e => new ProfilEtkinlikOzetViewModel
            {
                Id = e.Id,
                Baslik = e.Baslik,
                Tarih = e.Tarih,
                Kategori = e.Kategori,
                AdminOnaylandi = e.AdminOnaylandi,
                IptalEdildi = e.IptalEdildi
            }),
            OlusturduguEtkinlikler = olusturduguEtkinlikler.Select(e => new ProfilEtkinlikOzetViewModel
            {
                Id = e.Id,
                Baslik = e.Baslik,
                Tarih = e.Tarih,
                Kategori = e.Kategori,
                AdminOnaylandi = e.AdminOnaylandi,
                IptalEdildi = e.IptalEdildi
            })
        };

        return View(model);
    }

    // GET: /Account/ProfilDuzenle
    [Authorize]
    public async Task<IActionResult> ProfilDuzenle()
    {
        var uye = await _userManager.GetUserAsync(User);
        if (uye is null) return Challenge();

        var model = new ProfilDuzenleViewModel
        {
            Ad = uye.Ad,
            Soyad = uye.Soyad,
            Biyografi = uye.Biyografi
        };

        return View(model);
    }

    // POST: /Account/ProfilDuzenle
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProfilDuzenle(ProfilDuzenleViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var uye = await _userManager.GetUserAsync(User);
        if (uye is null) return Challenge();

        uye.Ad = model.Ad;
        uye.Soyad = model.Soyad;
        uye.Biyografi = model.Biyografi;

        await _userManager.UpdateAsync(uye);

        TempData["Mesaj"] = "Profiliniz başarıyla güncellendi.";
        TempData["MesajTipi"] = "success";
        return RedirectToAction(nameof(Profil));
    }

    // POST: /Account/Cikis
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cikis()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // Identity hata kodlarını Türkçeye çevirir
    private static string HataMesajiniCevir(string code) => code switch
    {
        "DuplicateEmail" => "Bu e-posta adresi zaten kayıtlı.",
        "DuplicateUserName" => "Bu kullanıcı adı zaten kullanılıyor.",
        "PasswordTooShort" => "Şifre en az 8 karakter olmalıdır.",
        "PasswordRequiresNonAlphanumeric" => "Şifre en az bir özel karakter içermelidir (!@#$%...).",
        "PasswordRequiresDigit" => "Şifre en az bir rakam içermelidir.",
        "PasswordRequiresUpper" => "Şifre en az bir büyük harf içermelidir.",
        "PasswordRequiresLower" => "Şifre en az bir küçük harf içermelidir.",
        _ => "Bir hata oluştu. Lütfen tekrar deneyin."
    };
}
