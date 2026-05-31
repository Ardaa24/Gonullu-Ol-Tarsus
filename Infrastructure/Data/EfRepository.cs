using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Enums;
using GonulluOlTarsus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GonulluOlTarsus.Infrastructure.Data;

/// <summary>
/// Generic repository implementasyonu — temel CRUD için.
/// </summary>
public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public EfRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    public void Update(T entity) =>
        _dbSet.Update(entity);

    public void Delete(T entity) =>
        _dbSet.Remove(entity);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}

/// <summary>
/// Etkinliğe özgü sorgu metotlarının EF Core implementasyonu.
/// </summary>
public class EfEtkinlikRepository : EfRepository<Etkinlik>, IEtkinlikRepository
{
    public EfEtkinlikRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Etkinlik>> GetOnaylananlarAsync() =>
        await _context.Etkinlikler
            .Include(e => e.Uye)
            .Include(e => e.Katilimlar)
            .Where(e => e.AdminOnaylandi && e.AktifMi && e.Tarih >= DateTime.UtcNow)
            .OrderBy(e => e.Tarih)
            .ToListAsync();

    public async Task<IEnumerable<Etkinlik>> GetByKategoriAsync(KategoriTip kategori) =>
        await _context.Etkinlikler
            .Include(e => e.Uye)
            .Include(e => e.Katilimlar)
            .Where(e => e.Kategori == kategori && e.AdminOnaylandi && e.AktifMi && e.Tarih >= DateTime.UtcNow)
            .OrderBy(e => e.Tarih)
            .ToListAsync();

    public async Task<IEnumerable<Etkinlik>> GetOnayBekleyenlerAsync() =>
        await _context.Etkinlikler
            .Include(e => e.Uye)
            .Where(e => !e.AdminOnaylandi && e.AktifMi)
            .OrderByDescending(e => e.OlusturulmaTarihi)
            .ToListAsync();

    public async Task<Etkinlik?> GetDetayliAsync(int id) =>
        await _context.Etkinlikler
            .Include(e => e.Uye)
            .Include(e => e.Katilimlar)
                .ThenInclude(k => k.Uye)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IEnumerable<Etkinlik>> GetUyeEtkinlikleriAsync(string uyeId) =>
        await _context.Etkinlikler
            .Include(e => e.Katilimlar)
            .Where(e => e.Katilimlar.Any(k => k.UyeId == uyeId) && e.AktifMi)
            .OrderByDescending(e => e.Tarih)
            .ToListAsync();
}
