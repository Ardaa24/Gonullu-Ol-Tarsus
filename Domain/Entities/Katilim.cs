namespace GonulluOlTarsus.Domain.Entities;

/// <summary>
/// Etkinlik katılım kaydı — Etkinlik ve Üye arasındaki köprü entity.
/// </summary>
public class Katilim
{
    public int Id { get; private set; }
    public int EtkinlikId { get; private set; }
    public string UyeId { get; private set; } = string.Empty;
    public DateTime KatilimTarihi { get; private set; } = DateTime.UtcNow;

    // Navigasyon özellikleri
    public Etkinlik? Etkinlik { get; private set; }
    public Uye? Uye { get; private set; }

    // EF Core için korumalı parametresiz constructor
    protected Katilim() { }

    /// <summary>
    /// Yeni katılım kaydı oluşturur.
    /// </summary>
    public Katilim(int etkinlikId, string uyeId)
    {
        EtkinlikId = etkinlikId;
        UyeId = uyeId;
        KatilimTarihi = DateTime.UtcNow;
    }
}
