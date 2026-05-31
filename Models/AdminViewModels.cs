using GonulluOlTarsus.Domain.Enums;

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
    public string Id { get; set; } = string.Empty;
    public string TamAd { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
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
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public string Konum { get; set; } = string.Empty;
    public KategoriTip Kategori { get; set; }
    public int Kontenjan { get; set; }
}
