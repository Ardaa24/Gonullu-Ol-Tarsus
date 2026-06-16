using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Enums;

namespace GonulluOlTarsus.Services.Abstract;

/// <summary>
/// Etkinlik iş mantığı operasyonlarını tanımlar.
/// Controller'lar bu arayüz üzerinden servis katmanına erişir.
/// </summary>
public interface IEtkinlikService
{
    Task<IEnumerable<Etkinlik>> GetOnaylananEtkinliklerAsync();
    Task<IEnumerable<Etkinlik>> GetKategoriyeGoreAsync(KategoriTip? kategori);
    Task<IEnumerable<Etkinlik>> GetOnayBekleyenlerAsync();
    Task<Etkinlik?> GetDetayAsync(int id);
    Task<Etkinlik> OlusturAsync(
        string baslik,
        string aciklama,
        DateTime tarih,
        string konum,
        KategoriTip kategori,
        int kontenjan,
        string uyeId);
    Task<bool> KatilAsync(int etkinlikId, string uyeId);
    Task<bool> KatilimIptalEtAsync(int etkinlikId, string uyeId);
    Task<bool> AdminOnaylaAsync(int etkinlikId);
    Task<bool> AdminOnayiGeriAlAsync(int etkinlikId);
    Task<bool> KullaniciIptalEtAsync(int etkinlikId, string uyeId);
    Task<bool> AdminEtkinlikSilAsync(int etkinlikId);
    Task<bool> AdminEtkinlikGuncelleAsync(int etkinlikId, string baslik, string aciklama, DateTime tarih, string konum, KategoriTip kategori, int kontenjan);
    Task<IEnumerable<Etkinlik>> GetTumEtkinliklerAdminAsync();
    Task<IEnumerable<Etkinlik>> GetUyeKatildigiEtkinliklerAsync(string uyeId);
    Task<IEnumerable<Etkinlik>> GetUyeEtkinlikleriAsync(string uyeId);
    Task<(int ToplamAktifEtkinlik, int OnayBekleyenSayisi, int ToplamKatilim)> GetDashboardIstatistikleriAsync();
}
