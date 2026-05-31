using GonulluOlTarsus.Domain.Enums;

namespace GonulluOlTarsus.Domain.Entities;

/// <summary>
/// Platform etkinliği — Rich Domain Model.
/// Business kuralları bu sınıf içinde kapsüllenmiştir.
/// </summary>
public class Etkinlik : BaseEntity
{
    public string Baslik { get; private set; } = string.Empty;
    public string Aciklama { get; private set; } = string.Empty;
    public DateTime Tarih { get; private set; }
    public string Konum { get; private set; } = string.Empty;
    public KategoriTip Kategori { get; private set; }
    public int Kontenjan { get; private set; }

    /// <summary>
    /// Etkinliğin admin tarafından onaylanıp onaylanmadığı.
    /// Yeni oluşturulan etkinlikler varsayılan olarak onay bekler.
    /// </summary>
    public bool AdminOnaylandi { get; private set; } = false;

    // Oluşturucu üye bilgisi
    public string UyeId { get; private set; } = string.Empty;
    public Uye? Uye { get; private set; }

    // Katılım koleksiyonu
    private readonly List<Katilim> _katilimlar = new();
    public IReadOnlyCollection<Katilim> Katilimlar => _katilimlar.AsReadOnly();

    // EF Core için korumalı parametresiz constructor
    protected Etkinlik() { }

    /// <summary>
    /// Yeni etkinlik oluşturur. Tüm zorunlu alanları parametre olarak alır.
    /// </summary>
    public static Etkinlik Olustur(
        string baslik,
        string aciklama,
        DateTime tarih,
        string konum,
        KategoriTip kategori,
        int kontenjan,
        string uyeId)
    {
        if (string.IsNullOrWhiteSpace(baslik))
            throw new ArgumentException("Etkinlik başlığı boş olamaz.", nameof(baslik));
        if (string.IsNullOrWhiteSpace(aciklama))
            throw new ArgumentException("Etkinlik açıklaması boş olamaz.", nameof(aciklama));
        if (tarih <= DateTime.UtcNow)
            throw new ArgumentException("Etkinlik tarihi gelecekte olmalıdır.", nameof(tarih));
        if (kontenjan < 1)
            throw new ArgumentException("Kontenjan en az 1 kişi olmalıdır.", nameof(kontenjan));

        return new Etkinlik
        {
            Baslik = baslik,
            Aciklama = aciklama,
            Tarih = tarih,
            Konum = konum,
            Kategori = kategori,
            Kontenjan = kontenjan,
            UyeId = uyeId
        };
    }

    // --- Business Metotları ---

    /// <summary>
    /// Etkinliğin kontenjanının dolup dolmadığını kontrol eder.
    /// </summary>
    public bool DoluMu() => _katilimlar.Count >= Kontenjan;

    /// <summary>
    /// Mevcut katılım sayısını döndürür.
    /// </summary>
    public int MevcutKatilimciSayisi() => _katilimlar.Count;

    /// <summary>
    /// Kalan kontenjanı döndürür.
    /// </summary>
    public int KalanKontenjan() => Kontenjan - _katilimlar.Count;

    /// <summary>
    /// Belirtilen üyenin bu etkinliğe zaten kayıtlı olup olmadığını kontrol eder.
    /// </summary>
    public bool UyeZatenKatildi(string uyeId) =>
        _katilimlar.Any(k => k.UyeId == uyeId);

    /// <summary>
    /// Üyeyi etkinliğe kaydeder.
    /// Business kurallarını kontrol eder: kontenjan, tekrar kayıt, geçmiş tarih.
    /// </summary>
    public Katilim EtkinligeKatil(string uyeId)
    {
        if (Tarih <= DateTime.UtcNow)
            throw new InvalidOperationException("Geçmiş tarihli etkinliklere katılım yapılamaz.");
        if (!AdminOnaylandi)
            throw new InvalidOperationException("Onaylanmamış etkinliklere katılım yapılamaz.");
        if (DoluMu())
            throw new InvalidOperationException("Etkinlik kontenjanı dolmuştur.");
        if (UyeZatenKatildi(uyeId))
            throw new InvalidOperationException("Bu etkinliğe zaten kayıtlısınız.");

        var katilim = new Katilim(Id, uyeId);
        _katilimlar.Add(katilim);
        GuncellenmeTarihiniAyarla();
        return katilim;
    }

    /// <summary>
    /// Üyenin etkinlik katılımını iptal eder.
    /// </summary>
    public void KatilimIptalEt(string uyeId)
    {
        var katilim = _katilimlar.FirstOrDefault(k => k.UyeId == uyeId)
            ?? throw new InvalidOperationException("Bu etkinliğe kayıtlı değilsiniz.");

        _katilimlar.Remove(katilim);
        GuncellenmeTarihiniAyarla();
    }

    /// <summary>
    /// Admin tarafından etkinliği onaylar.
    /// </summary>
    public void AdminOnayla()
    {
        AdminOnaylandi = true;
        GuncellenmeTarihiniAyarla();
    }

    /// <summary>
    /// Admin tarafından etkinlik onayını geri alır.
    /// </summary>
    public void AdminOnayiGeriAl()
    {
        AdminOnaylandi = false;
        GuncellenmeTarihiniAyarla();
    }

    /// <summary>
    /// Etkinlik bilgilerini günceller.
    /// </summary>
    public void Guncelle(
        string baslik,
        string aciklama,
        DateTime tarih,
        string konum,
        KategoriTip kategori,
        int kontenjan)
    {
        if (string.IsNullOrWhiteSpace(baslik))
            throw new ArgumentException("Başlık boş olamaz.", nameof(baslik));
        if (kontenjan < _katilimlar.Count)
            throw new ArgumentException($"Kontenjan mevcut katılımcı sayısından ({_katilimlar.Count}) küçük olamaz.");

        Baslik = baslik;
        Aciklama = aciklama;
        Tarih = tarih;
        Konum = konum;
        Kategori = kategori;
        Kontenjan = kontenjan;
        GuncellenmeTarihiniAyarla();
    }
}
