namespace GonulluOlTarsus.Domain.Enums;

/// <summary>
/// Kategori enum değerleri için ortak yardımcı metotlar.
/// ViewModel'lardaki tekrar eden switch ifadelerini ortadan kaldırır (DRY).
/// </summary>
public static class KategoriHelper
{
    public static string KategoriAdiGetir(KategoriTip kategori) => kategori switch
    {
        KategoriTip.Cevre => "Çevre",
        KategoriTip.Barinak => "Barınak",
        KategoriTip.Egitim => "Eğitim",
        KategoriTip.YasliBakimi => "Yaşlı Bakımı",
        KategoriTip.Diger => "Diğer",
        _ => "Bilinmiyor"
    };

    public static string KategoriEmojiGetir(KategoriTip kategori) => kategori switch
    {
        KategoriTip.Cevre => "🌿",
        KategoriTip.Barinak => "🐾",
        KategoriTip.Egitim => "📚",
        KategoriTip.YasliBakimi => "💛",
        KategoriTip.Diger => "🤝",
        _ => "🤝"
    };

    public static string KategoriRenkSinifiGetir(KategoriTip kategori) => kategori switch
    {
        KategoriTip.Cevre => "badge-cevre",
        KategoriTip.Barinak => "badge-barinak",
        KategoriTip.Egitim => "badge-egitim",
        KategoriTip.YasliBakimi => "badge-yasli",
        KategoriTip.Diger => "badge-diger",
        _ => "badge-diger"
    };
}
