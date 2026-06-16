using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GonulluOlTarsus.Infrastructure.Data;

/// <summary>
/// Yorum operasyonlarının EF Core implementasyonu.
/// </summary>
public class EfYorumRepository : IYorumRepository
{
    private readonly AppDbContext _context;

    public EfYorumRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Yorum>> GetEtkinlikYorumlariAsync(int etkinlikId) =>
        await _context.Yorumlar
            .Include(y => y.Uye)
            .Where(y => y.EtkinlikId == etkinlikId && y.AktifMi)
            .OrderByDescending(y => y.OlusturulmaTarihi)
            .ToListAsync();

    public async Task AddAsync(Yorum yorum) =>
        await _context.Yorumlar.AddAsync(yorum);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
