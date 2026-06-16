using GonulluOlTarsus.Domain.Entities;

namespace GonulluOlTarsus.Domain.Interfaces;

/// <summary>
/// Yorum veritabanı operasyonlarını tanımlar.
/// </summary>
public interface IYorumRepository
{
    Task<IEnumerable<Yorum>> GetEtkinlikYorumlariAsync(int etkinlikId);
    Task AddAsync(Yorum yorum);
    Task SaveChangesAsync();
}
