using GonulluOlTarsus.Domain.Enums;
using GonulluOlTarsus.Models;
using GonulluOlTarsus.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace GonulluOlTarsus.Controllers;

public class HomeController : Controller
{
    private readonly IEtkinlikService _etkinlikService;

    public HomeController(IEtkinlikService etkinlikService)
    {
        _etkinlikService = etkinlikService;
    }

    public async Task<IActionResult> Index(KategoriTip? kategori)
    {
        var etkinlikler = await _etkinlikService.GetKategoriyeGoreAsync(kategori);

        var model = new EtkinlikListeViewModel
        {
            Etkinlikler = etkinlikler.Select(e => new EtkinlikOzetViewModel
            {
                Id = e.Id,
                Baslik = e.Baslik,
                Aciklama = e.Aciklama.Length > 150
                    ? e.Aciklama[..150] + "..."
                    : e.Aciklama,
                Tarih = e.Tarih,
                Konum = e.Konum,
                Kategori = e.Kategori,
                Kontenjan = e.Kontenjan,
                MevcutKatilimciSayisi = e.MevcutKatilimciSayisi(),
                DoluMu = e.DoluMu(),
                IptalEdildi = e.IptalEdildi,
                OlusturanAdSoyad = e.Uye?.TamAd ?? "Platform"
            }),
            SecilenKategori = kategori,
            ToplamEtkinlik = etkinlikler.Count()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
