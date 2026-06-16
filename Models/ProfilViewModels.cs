using System.ComponentModel.DataAnnotations;
using GonulluOlTarsus.Domain.Enums;

namespace GonulluOlTarsus.Models;

/// <summary>
/// Kullanıcı profil sayfası ViewModel.
/// Kullanıcı bilgileri, katıldığı ve oluşturduğu etkinlikleri içerir.
/// </summary>
public class ProfilViewModel
{
    public string Id { get; set; } = string.Empty;
    public string TamAd { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Biyografi { get; set; }
    public DateTime KayitTarihi { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool KendiProfili { get; set; }

    public IEnumerable<ProfilEtkinlikOzetViewModel> KatildigiEtkinlikler { get; set; } = Enumerable.Empty<ProfilEtkinlikOzetViewModel>();
    public IEnumerable<ProfilEtkinlikOzetViewModel> OlusturduguEtkinlikler { get; set; } = Enumerable.Empty<ProfilEtkinlikOzetViewModel>();

    public int ToplamKatilim => KatildigiEtkinlikler.Count();
    public int ToplamOlusturulan => OlusturduguEtkinlikler.Count();
}

/// <summary>
/// Profil sayfasındaki etkinlik özeti.
/// </summary>
public class ProfilEtkinlikOzetViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public KategoriTip Kategori { get; set; }
    public bool AdminOnaylandi { get; set; }
    public bool IptalEdildi { get; set; }
    public bool Gecmis => Tarih <= DateTime.UtcNow;

    public string KategoriAdi => GonulluOlTarsus.Domain.Enums.KategoriHelper.KategoriAdiGetir(Kategori);
    public string KategoriEmoji => GonulluOlTarsus.Domain.Enums.KategoriHelper.KategoriEmojiGetir(Kategori);
}

/// <summary>
/// Profil düzenleme formu ViewModel.
/// </summary>
public class ProfilDuzenleViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ad 2-50 karakter arasında olmalıdır.")]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyad 2-50 karakter arasında olmalıdır.")]
    [Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Biyografi en fazla 500 karakter olabilir.")]
    [Display(Name = "Hakkımda")]
    public string? Biyografi { get; set; }
}
