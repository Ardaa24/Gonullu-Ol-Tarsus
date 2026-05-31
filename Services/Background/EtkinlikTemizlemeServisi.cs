using GonulluOlTarsus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GonulluOlTarsus.Services.Background;

/// <summary>
/// İptal edilmiş etkinlikleri belirli aralıklarla (ör. saat başı) kontrol eden 
/// ve iptal tarihinin üzerinden 24 saat geçmişse tamamen sistemden silen (soft-delete ile Pasifles())
/// arka plan servisidir.
/// </summary>
public class EtkinlikTemizlemeServisi : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EtkinlikTemizlemeServisi> _logger;
    private readonly TimeSpan _kontrolAraligi = TimeSpan.FromHours(1);

    public EtkinlikTemizlemeServisi(IServiceProvider serviceProvider, ILogger<EtkinlikTemizlemeServisi> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EtkinlikTemizlemeServisi çalışmaya başladı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await TemizligiYapAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EtkinlikTemizlemeServisi çalışırken bir hata oluştu.");
            }

            // Bir sonraki kontrole kadar bekle
            await Task.Delay(_kontrolAraligi, stoppingToken);
        }

        _logger.LogInformation("EtkinlikTemizlemeServisi durduruluyor.");
    }

    private async Task TemizligiYapAsync(CancellationToken stoppingToken)
    {
        // BackgroundService singleton çalıştığı için DbContext (scoped) kullanırken scope açmalıyız.
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // İptal edilmiş, hala aktif görünen ve iptalinin üzerinden 24 saat geçmiş etkinlikleri bul
        var silinecekTarihSiniri = DateTime.UtcNow.AddDays(-1);

        var silinecekEtkinlikler = await context.Etkinlikler
            .Where(e => e.IptalEdildi && e.AktifMi && e.IptalTarihi.HasValue && e.IptalTarihi.Value <= silinecekTarihSiniri)
            .ToListAsync(stoppingToken);

        if (silinecekEtkinlikler.Any())
        {
            foreach (var etkinlik in silinecekEtkinlikler)
            {
                etkinlik.Pasifles(); // BaseEntity üzerinden AktifMi = false yapılıyor (Soft Delete)
            }

            await context.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("{Sayi} adet iptal edilmiş etkinlik arka planda sistemden (soft delete) silindi.", silinecekEtkinlikler.Count);
        }
    }
}
