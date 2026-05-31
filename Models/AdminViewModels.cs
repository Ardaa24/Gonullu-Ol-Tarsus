using GonulluOlTarsus.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GonulluOlTarsus.Models;

public class AdminDashboardViewModel
{
    public int ToplamUyeSayisi { get; set; }
    public int ToplamEtkinlikSayisi { get; set; }
    public int OnayBekleyenEtkinlikSayisi { get; set; }
    public int ToplamKatilimSayisi { get; set; }
    
    // Grafikler için veriler (örnek olarak son 7 günün üye kayıtları)
    public List<string> Son7GunEtiketleri { get; set; } = new();
    public List<int> Son7GunUyeKayitlari { get; set; } = new();
    public List<int> Son7GunEtkinlikleri { get; set; } = new();
}

public class AdminKullaniciViewModel
{
    public string Id { get; set; } = string.Empty;
    public string TamAd { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}

public class AdminKullaniciDuzenleViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Ad Soyad 3-100 karakter arasında olmalıdır.")]
    [Display(Name = "Ad Soyad")]
    public string TamAd { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rol alanı zorunludur.")]
    [RegularExpression(@"^(Gonullu|Admin|Super Admin)$", ErrorMessage = "Geçersiz bir rol seçildi.")]
    [Display(Name = "Rol")]
    public string Rol { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Yeni şifre en az 8 karakter olmalıdır.")]
    [Display(Name = "Yeni Şifre")]
    public string? YeniSifre { get; set; } // Boş bırakılırsa değişmez
}

public class AdminEtkinlikListeViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Olusturan { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public int KatilimciSayisi { get; set; }
    public int Kontenjan { get; set; }
    public bool AdminOnaylandi { get; set; }
    public bool IptalEdildi { get; set; }
}

public class AdminEtkinlikDuzenleViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Başlık 5-200 karakter arasında olmalıdır.")]
    [Display(Name = "Etkinlik Başlığı")]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(4000, MinimumLength = 20, ErrorMessage = "Açıklama 20-4000 karakter arasında olmalıdır.")]
    [Display(Name = "Açıklama")]
    public string Aciklama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tarih zorunludur.")]
    [Display(Name = "Etkinlik Tarihi")]
    public DateTime Tarih { get; set; }

    [Required(ErrorMessage = "Konum zorunludur.")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "Konum 3-300 karakter arasında olmalıdır.")]
    [Display(Name = "Konum / Adres")]
    public string Konum { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori seçiniz.")]
    [Display(Name = "Etkinlik Kategorisi")]
    public KategoriTip Kategori { get; set; }

    [Required(ErrorMessage = "Kontenjan zorunludur.")]
    [Range(1, 1000, ErrorMessage = "Kontenjan 1-1000 arasında olmalıdır.")]
    [Display(Name = "Kontenjan (Kişi Sayısı)")]
    public int Kontenjan { get; set; }
}
