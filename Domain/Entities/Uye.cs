using Microsoft.AspNetCore.Identity;

namespace GonulluOlTarsus.Domain.Entities;

/// <summary>
/// Platforma kayıtlı üye. IdentityUser'ı genişleterek
/// Tarsus platformuna özgü profil alanları ekler.
/// </summary>
public class Uye : IdentityUser
{
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? Biyografi { get; set; }
    public DateTime KayitTarihi { get; init; } = DateTime.UtcNow;
    public bool Onayli { get; private set; } = true;

    // Navigasyon özellikleri
    public ICollection<Katilim> Katilimlar { get; private set; } = new List<Katilim>();
    public ICollection<Etkinlik> OlusturulanEtkinlikler { get; private set; } = new List<Etkinlik>();

    /// <summary>
    /// Üyenin tam adını döndürür.
    /// </summary>
    public string TamAd => $"{Ad} {Soyad}".Trim();
}
