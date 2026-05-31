using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Enums;
using GonulluOlTarsus.Domain.Interfaces;
using GonulluOlTarsus.Services.Abstract;
using Microsoft.Extensions.Logging;

namespace GonulluOlTarsus.Services.Concrete;

/// <summary>
/// Etkinlik iş mantığının somut implementasyonu.
/// Domain model metotlarına delege ederek business kurallarını uygular.
/// </summary>
public class EtkinlikService : IEtkinlikService
{
    private readonly IEtkinlikRepository _etkinlikRepo;
    private readonly ILogger<EtkinlikService> _logger;

    public EtkinlikService(IEtkinlikRepository etkinlikRepo, ILogger<EtkinlikService> logger)
    {
        _etkinlikRepo = etkinlikRepo;
        _logger = logger;
    }

    public async Task<IEnumerable<Etkinlik>> GetOnaylananEtkinliklerAsync() =>
        await _etkinlikRepo.GetOnaylananlarAsync();

    public async Task<IEnumerable<Etkinlik>> GetKategoriyeGoreAsync(KategoriTip? kategori)
    {
        if (kategori.HasValue)
            return await _etkinlikRepo.GetByKategoriAsync(kategori.Value);

        return await _etkinlikRepo.GetOnaylananlarAsync();
    }

    public async Task<IEnumerable<Etkinlik>> GetOnayBekleyenlerAsync() =>
        await _etkinlikRepo.GetOnayBekleyenlerAsync();

    public async Task<Etkinlik?> GetDetayAsync(int id) =>
        await _etkinlikRepo.GetDetayliAsync(id);

    public async Task<Etkinlik> OlusturAsync(
        string baslik,
        string aciklama,
        DateTime tarih,
        string konum,
        KategoriTip kategori,
        int kontenjan,
        string uyeId)
    {
        // Domain model business kurallarını devreye sokar (fırlatan exception'lar burada da geçerli)
        var etkinlik = Etkinlik.Olustur(baslik, aciklama, tarih, konum, kategori, kontenjan, uyeId);

        await _etkinlikRepo.AddAsync(etkinlik);
        await _etkinlikRepo.SaveChangesAsync();

        _logger.LogInformation("Yeni etkinlik oluşturuldu: {EtkinlikId} - {Baslik}", etkinlik.Id, etkinlik.Baslik);
        return etkinlik;
    }

    public async Task<bool> KatilAsync(int etkinlikId, string uyeId)
    {
        var etkinlik = await _etkinlikRepo.GetDetayliAsync(etkinlikId);
        if (etkinlik is null)
        {
            _logger.LogWarning("Katılım başarısız — Etkinlik bulunamadı: {EtkinlikId}", etkinlikId);
            return false;
        }

        try
        {
            etkinlik.EtkinligeKatil(uyeId);
            _etkinlikRepo.Update(etkinlik);
            await _etkinlikRepo.SaveChangesAsync();

            _logger.LogInformation("Katılım kaydedildi: Üye={UyeId}, Etkinlik={EtkinlikId}", uyeId, etkinlikId);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Katılım reddedildi: {Sebep}", ex.Message);
            return false;
        }
    }

    public async Task<bool> KatilimIptalEtAsync(int etkinlikId, string uyeId)
    {
        var etkinlik = await _etkinlikRepo.GetDetayliAsync(etkinlikId);
        if (etkinlik is null) return false;

        try
        {
            etkinlik.KatilimIptalEt(uyeId);
            _etkinlikRepo.Update(etkinlik);
            await _etkinlikRepo.SaveChangesAsync();

            _logger.LogInformation("Katılım iptal edildi: Üye={UyeId}, Etkinlik={EtkinlikId}", uyeId, etkinlikId);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("İptal reddedildi: {Sebep}", ex.Message);
            return false;
        }
    }

    public async Task<bool> AdminOnaylaAsync(int etkinlikId)
    {
        var etkinlik = await _etkinlikRepo.GetByIdAsync(etkinlikId);
        if (etkinlik is null) return false;

        etkinlik.AdminOnayla();
        _etkinlikRepo.Update(etkinlik);
        await _etkinlikRepo.SaveChangesAsync();

        _logger.LogInformation("Etkinlik admin tarafından onaylandı: {EtkinlikId}", etkinlikId);
        return true;
    }

    public async Task<bool> AdminOnayiGeriAlAsync(int etkinlikId)
    {
        var etkinlik = await _etkinlikRepo.GetByIdAsync(etkinlikId);
        if (etkinlik is null) return false;

        etkinlik.AdminOnayiGeriAl();
        _etkinlikRepo.Update(etkinlik);
        await _etkinlikRepo.SaveChangesAsync();

        return true;
    }

    public async Task<bool> KullaniciIptalEtAsync(int etkinlikId, string uyeId)
    {
        var etkinlik = await _etkinlikRepo.GetByIdAsync(etkinlikId);
        if (etkinlik is null) return false;

        try
        {
            etkinlik.KullaniciIptalEt(uyeId);
            _etkinlikRepo.Update(etkinlik);
            await _etkinlikRepo.SaveChangesAsync();

            _logger.LogInformation("Etkinlik iptal edildi: Etkinlik={EtkinlikId}, Uye={UyeId}", etkinlikId, uyeId);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Etkinlik iptali reddedildi: {Sebep}", ex.Message);
            return false;
        }
    }

    public async Task<bool> AdminEtkinlikSilAsync(int etkinlikId)
    {
        var etkinlik = await _etkinlikRepo.GetByIdAsync(etkinlikId);
        if (etkinlik is null) return false;

        etkinlik.Pasifles();
        _etkinlikRepo.Update(etkinlik);
        await _etkinlikRepo.SaveChangesAsync();

        _logger.LogInformation("Etkinlik admin tarafından silindi (soft delete): {EtkinlikId}", etkinlikId);
        return true;
    }

    public async Task<bool> AdminEtkinlikGuncelleAsync(int etkinlikId, string baslik, string aciklama, DateTime tarih, string konum, KategoriTip kategori, int kontenjan)
    {
        var etkinlik = await _etkinlikRepo.GetDetayliAsync(etkinlikId);
        if (etkinlik is null) return false;

        try
        {
            etkinlik.Guncelle(baslik, aciklama, tarih, konum, kategori, kontenjan);
            _etkinlikRepo.Update(etkinlik);
            await _etkinlikRepo.SaveChangesAsync();

            _logger.LogInformation("Etkinlik admin tarafından güncellendi: {EtkinlikId}", etkinlikId);
            return true;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Admin güncellemesi reddedildi: {Sebep}", ex.Message);
            return false;
        }
    }

    public async Task<IEnumerable<Etkinlik>> GetTumEtkinliklerAdminAsync() =>
        await _etkinlikRepo.GetTumEtkinliklerAdminAsync();
}
