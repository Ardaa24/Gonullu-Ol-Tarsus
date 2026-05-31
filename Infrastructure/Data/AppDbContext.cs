using GonulluOlTarsus.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GonulluOlTarsus.Infrastructure.Data;

/// <summary>
/// Uygulamanın birincil veritabanı bağlamı.
/// IdentityDbContext üzerinden hem kimlik doğrulama hem
/// de domain tabloları tek context'te yönetilir.
/// </summary>
public class AppDbContext : IdentityDbContext<Uye>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Etkinlik> Etkinlikler => Set<Etkinlik>();
    public DbSet<Katilim> Katilimlar => Set<Katilim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Etkinlik Konfigürasyonu ---
        modelBuilder.Entity<Etkinlik>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Baslik)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(e => e.Aciklama)
                  .IsRequired()
                  .HasMaxLength(4000);
            entity.Property(e => e.Konum)
                  .IsRequired()
                  .HasMaxLength(300);
            entity.Property(e => e.Kategori)
                  .IsRequired();
            entity.Property(e => e.Kontenjan)
                  .IsRequired();

            // Etkinlik → Uye (Oluşturucu) ilişkisi
            entity.HasOne(e => e.Uye)
                  .WithMany(u => u.OlusturulanEtkinlikler)
                  .HasForeignKey(e => e.UyeId)
                  .OnDelete(DeleteBehavior.Restrict);

            // EF Core'un private _katilimlar listesine erişimini sağla
            entity.HasMany(e => e.Katilimlar)
                  .WithOne(k => k.Etkinlik)
                  .HasForeignKey(k => k.EtkinlikId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Private backing field kullanımı
            entity.Navigation(e => e.Katilimlar).HasField("_katilimlar");
        });

        // --- Katilim Konfigürasyonu ---
        modelBuilder.Entity<Katilim>(entity =>
        {
            entity.HasKey(k => k.Id);

            // Bir üye aynı etkinliğe iki kez kaydolamaz (Unique index)
            entity.HasIndex(k => new { k.EtkinlikId, k.UyeId }).IsUnique();

            entity.HasOne(k => k.Uye)
                  .WithMany(u => u.Katilimlar)
                  .HasForeignKey(k => k.UyeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Uye Tablo Adı Düzenleme ---
        modelBuilder.Entity<Uye>().ToTable("Uyeler");
    }
}
