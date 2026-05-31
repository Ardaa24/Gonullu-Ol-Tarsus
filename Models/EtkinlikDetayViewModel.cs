using GonulluOlTarsus.Domain.Enums;

namespace GonulluOlTarsus.Models;

/// <summary>
/// Etkinlik detay sayfası için ViewModel.
/// Katılımcı listesi ve kullanıcı durumunu içerir.
/// </summary>
public class EtkinlikDetayViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public string Konum { get; set; } = string.Empty;
    public KategoriTip Kategori { get; set; }
    public int Kontenjan { get; set; }
    public bool AdminOnaylandi { get; set; }
    public string OlusturanAdSoyad { get; set; } = string.Empty;
    public DateTime OlusturulmaTarihi { get; set; }

    public IEnumerable<KatilimciViewModel> Katilimcilar { get; set; } = Enumerable.Empty<KatilimciViewModel>();

    // Oturum açık kullanıcıya göre durum
    public bool KullaniciGirisYapti { get; set; }
    public bool KullaniciZatenKatildi { get; set; }
    public bool EtkinlikDolu { get; set; }
    public bool EtkinlikGecmis { get; set; }

    public int KatilimciSayisi => Katilimcilar.Count();
    public int KalanKontenjan => Kontenjan - KatilimciSayisi;

    public string KategoriAdi => Kategori switch
    {
        KategoriTip.Cevre => "Çevre",
        KategoriTip.Barinak => "Barınak",
        KategoriTip.Egitim => "Eğitim",
        KategoriTip.YasliBakimi => "Yaşlı Bakımı",
        KategoriTip.Diger => "Diğer",
        _ => "Bilinmiyor"
    };

    public string KategoriEmoji => Kategori switch
    {
        KategoriTip.Cevre => "🌿",
        KategoriTip.Barinak => "🐾",
        KategoriTip.Egitim => "📚",
        KategoriTip.YasliBakimi => "💛",
        KategoriTip.Diger => "🤝",
        _ => "🤝"
    };
}

/// <summary>
/// Katılımcı listesindeki tek bir üyenin özet bilgisi.
/// </summary>
public class KatilimciViewModel
{
    public string AdSoyad { get; set; } = string.Empty;
    public DateTime KatilimTarihi { get; set; }
}
