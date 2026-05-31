using GonulluOlTarsus.Domain.Enums;

namespace GonulluOlTarsus.Models;

/// <summary>
/// Ana sayfa etkinlik listesi ve filtreleme için ViewModel.
/// </summary>
public class EtkinlikListeViewModel
{
    public IEnumerable<EtkinlikOzetViewModel> Etkinlikler { get; set; } = Enumerable.Empty<EtkinlikOzetViewModel>();
    public KategoriTip? SecilenKategori { get; set; }
    public int ToplamEtkinlik { get; set; }
}

/// <summary>
/// Liste görünümünde tek bir etkinlik kartı için veri taşıyıcı.
/// </summary>
public class EtkinlikOzetViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public string Konum { get; set; } = string.Empty;
    public KategoriTip Kategori { get; set; }
    public int Kontenjan { get; set; }
    public int MevcutKatilimciSayisi { get; set; }
    public bool DoluMu { get; set; }
    public bool IptalEdildi { get; set; }
    public string OlusturanAdSoyad { get; set; } = string.Empty;

    public int KalanKontenjan => Kontenjan - MevcutKatilimciSayisi;

    public string KategoriAdi => Kategori switch
    {
        KategoriTip.Cevre => "Çevre",
        KategoriTip.Barinak => "Barınak",
        KategoriTip.Egitim => "Eğitim",
        KategoriTip.YasliBakimi => "Yaşlı Bakımı",
        KategoriTip.Diger => "Diğer",
        _ => "Bilinmiyor"
    };

    public string KategoriRenkSinifi => Kategori switch
    {
        KategoriTip.Cevre => "badge-cevre",
        KategoriTip.Barinak => "badge-barinak",
        KategoriTip.Egitim => "badge-egitim",
        KategoriTip.YasliBakimi => "badge-yasli",
        KategoriTip.Diger => "badge-diger",
        _ => "badge-diger"
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
