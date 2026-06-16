using GonulluOlTarsus.Domain.Entities;

namespace GonulluOlTarsus.Services.Abstract;

/// <summary>
/// Yorum iş mantığı operasyonlarını tanımlar.
/// </summary>
public interface IYorumService
{
    Task<IEnumerable<Yorum>> GetEtkinlikYorumlariAsync(int etkinlikId);
    Task<Yorum> YorumEkleAsync(string icerik, int etkinlikId, string uyeId);
}
