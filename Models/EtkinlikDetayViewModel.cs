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
    public bool IptalEdildi { get; set; }
    public string OlusturanAdSoyad { get; set; } = string.Empty;
    public DateTime OlusturulmaTarihi { get; set; }

    public IEnumerable<KatilimciViewModel> Katilimcilar { get; set; } = Enumerable.Empty<KatilimciViewModel>();

    // Oturum açık kullanıcıya göre durum
    public bool KullaniciGirisYapti { get; set; }
    public bool KullaniciZatenKatildi { get; set; }
    public bool KullaniciKendiEtkinligi { get; set; }
    public bool EtkinlikDolu { get; set; }
    public bool EtkinlikGecmis { get; set; }

    public int KatilimciSayisi => Katilimcilar.Count();
    public int KalanKontenjan => Kontenjan - KatilimciSayisi;

    public IEnumerable<YorumViewModel> Yorumlar { get; set; } = Enumerable.Empty<YorumViewModel>();
    public int YorumSayisi => Yorumlar.Count();

    public string KategoriAdi => KategoriHelper.KategoriAdiGetir(Kategori);
    public string KategoriEmoji => KategoriHelper.KategoriEmojiGetir(Kategori);
}

/// <summary>
/// Katılımcı listesindeki tek bir üyenin özet bilgisi.
/// </summary>
public class KatilimciViewModel
{
    public string AdSoyad { get; set; } = string.Empty;
    public DateTime KatilimTarihi { get; set; }
}

/// <summary>
/// Etkinlik detay sayfasındaki tek bir yorum.
/// </summary>
public class YorumViewModel
{
    public int Id { get; set; }
    public string Icerik { get; set; } = string.Empty;
    public string YazarAd { get; set; } = string.Empty;
    public DateTime YazilmaTarihi { get; set; }
    public bool KendiYorumu { get; set; }
}
