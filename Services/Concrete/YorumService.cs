using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Interfaces;
using GonulluOlTarsus.Services.Abstract;
using Microsoft.Extensions.Logging;

namespace GonulluOlTarsus.Services.Concrete;

/// <summary>
/// Yorum iş mantığının somut implementasyonu.
/// </summary>
public class YorumService : IYorumService
{
    private readonly IYorumRepository _yorumRepo;
    private readonly ILogger<YorumService> _logger;

    public YorumService(IYorumRepository yorumRepo, ILogger<YorumService> logger)
    {
        _yorumRepo = yorumRepo;
        _logger = logger;
    }

    public async Task<IEnumerable<Yorum>> GetEtkinlikYorumlariAsync(int etkinlikId) =>
        await _yorumRepo.GetEtkinlikYorumlariAsync(etkinlikId);

    public async Task<Yorum> YorumEkleAsync(string icerik, int etkinlikId, string uyeId)
    {
        var yorum = Yorum.Olustur(icerik, etkinlikId, uyeId);
        await _yorumRepo.AddAsync(yorum);
        await _yorumRepo.SaveChangesAsync();

        _logger.LogInformation("Yeni yorum eklendi: Etkinlik={EtkinlikId}, Uye={UyeId}", etkinlikId, uyeId);
        return yorum;
    }
}
