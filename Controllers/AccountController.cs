using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GonulluOlTarsus.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<Uye> _userManager;
    private readonly SignInManager<Uye> _signInManager;

    public AccountController(UserManager<Uye> userManager, SignInManager<Uye> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
