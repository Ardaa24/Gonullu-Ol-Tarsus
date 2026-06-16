namespace GonulluOlTarsus.Domain.Entities;

/// <summary>
/// Etkinlik yorumu — Gönüllüler etkinlik sahibine soru sorabilir veya yorum bırakabilir.
/// </summary>
public class Yorum : BaseEntity
{
    public string Icerik { get; private set; } = string.Empty;
    public int EtkinlikId { get; private set; }
    public string UyeId { get; private set; } = string.Empty;

    // Navigasyon özellikleri
    public Etkinlik? Etkinlik { get; private set; }
    public Uye? Uye { get; private set; }

    // EF Core için korumalı parametresiz constructor
    protected Yorum() { }

    /// <summary>
    /// Yeni yorum oluşturur.
    /// </summary>
    public static Yorum Olustur(string icerik, int etkinlikId, string uyeId)
    {
        if (string.IsNullOrWhiteSpace(icerik))
            throw new ArgumentException("Yorum içeriği boş olamaz.", nameof(icerik));
        if (icerik.Length > 1000)
            throw new ArgumentException("Yorum en fazla 1000 karakter olabilir.", nameof(icerik));

        return new Yorum
        {
            Icerik = icerik,
            EtkinlikId = etkinlikId,
            UyeId = uyeId
        };
    }
}
