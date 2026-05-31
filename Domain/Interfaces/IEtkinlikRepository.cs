using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Enums;

namespace GonulluOlTarsus.Domain.Interfaces;

/// <summary>
/// Generic repository arayüzü — temel CRUD operasyonları.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}

/// <summary>
/// Etkinliğe özgü sorgu metotlarını tanımlar.
/// </summary>
public interface IEtkinlikRepository : IRepository<Etkinlik>
{
    Task<IEnumerable<Etkinlik>> GetOnaylananlarAsync();
    Task<IEnumerable<Etkinlik>> GetByKategoriAsync(KategoriTip kategori);
    Task<IEnumerable<Etkinlik>> GetOnayBekleyenlerAsync();
    Task<Etkinlik?> GetDetayliAsync(int id); // Katilimlar ve Uye dahil
    Task<IEnumerable<Etkinlik>> GetUyeEtkinlikleriAsync(string uyeId);
    Task<IEnumerable<Etkinlik>> GetTumEtkinliklerAdminAsync();
}
