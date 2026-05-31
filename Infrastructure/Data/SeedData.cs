using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GonulluOlTarsus.Infrastructure.Data;

/// <summary>
/// Uygulama ilk çalıştırıldığında örnek veri oluşturur.
/// Admin kullanıcısı ve örnek etkinlikler seed edilir.
/// </summary>
public static class SeedData
{
    private const string AdminEmail = "admin@gonulluoltarsus.com";
    private const string AdminSifre = "Admin@1234!";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Uye>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Migrasyon uygula
        await context.Database.MigrateAsync();

        // Rolleri oluştur
        await RollerOlustur(roleManager);

        // Admin kullanıcısını oluştur
        var admin = await AdminOlustur(userManager);

        // Örnek etkinlikleri ekle
        await EtkinlikleriEkle(context, admin.Id);
    }

    private static async Task RollerOlustur(RoleManager<IdentityRole> roleManager)
    {
        string[] roller = ["Admin", "Gonullu"];
        foreach (var rol in roller)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }
    }

    private static async Task<Uye> AdminOlustur(UserManager<Uye> userManager)
    {
        var mevcutAdmin = await userManager.FindByEmailAsync(AdminEmail);
        if (mevcutAdmin != null) return mevcutAdmin;

        var admin = new Uye
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            Ad = "Platform",
            Soyad = "Yöneticisi",
            EmailConfirmed = true,
            Biyografi = "Gönüllü Ol | Tarsus platform yöneticisi.",
            KayitTarihi = DateTime.UtcNow
        };

        var sonuc = await userManager.CreateAsync(admin, AdminSifre);
        if (sonuc.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");

        return admin;
    }

    private static async Task EtkinlikleriEkle(AppDbContext context, string adminId)
    {
        if (await context.Etkinlikler.AnyAsync()) return;

        var etkinlikler = new[]
        {
            Etkinlik.Olustur(
                baslik: "Berdan Nehri Temizlik Kampanyası",
                aciklama: "Tarsus'un simgesi Berdan Nehri kıyısındaki plastik atıkları topluyoruz. Eldiven ve poşet tarafımızdan sağlanacak. Aile dostu bir etkinliktir.",
                tarih: DateTime.UtcNow.AddDays(7),
                konum: "Berdan Nehri Piknik Alanı, Tarsus",
                kategori: KategoriTip.Cevre,
                kontenjan: 50,
                uyeId: adminId),

            Etkinlik.Olustur(
                baslik: "Tarsus Belediyesi Barınak Ziyareti",
                aciklama: "Tarsus Belediyesi hayvan barınağındaki dostlarımızı ziyaret ediyoruz. Mama getirmeyi unutmayın! Veteriner eşliğinde sağlık kontrolü de yapılacak.",
                tarih: DateTime.UtcNow.AddDays(5),
                konum: "Tarsus Belediyesi Hayvan Barınağı",
                kategori: KategoriTip.Barinak,
                kontenjan: 20,
                uyeId: adminId),

            Etkinlik.Olustur(
                baslik: "İlkokul Öğrencilerine Fen Bilimleri Desteği",
                aciklama: "Tarsus Üniversitesi gönüllü öğrencileri olarak ilkokul çocuklarına eğlenceli deneyler yaparak fen bilimlerini sevdiriyoruz.",
                tarih: DateTime.UtcNow.AddDays(10),
                konum: "Atatürk İlkokulu, Tarsus",
                kategori: KategoriTip.Egitim,
                kontenjan: 15,
                uyeId: adminId),

            Etkinlik.Olustur(
                baslik: "Huzurevinde Neşe Günü",
                aciklama: "Tarsus Huzurevi sakinleriyle birlikte müzik dinleyip sohbet ediyoruz. Müzik aleti çalabilenler gelebilir, yoksa sadece sohbet de yeterli!",
                tarih: DateTime.UtcNow.AddDays(14),
                konum: "Tarsus Huzurevi, Merkez",
                kategori: KategoriTip.YasliBakimi,
                kontenjan: 30,
                uyeId: adminId),

            Etkinlik.Olustur(
                baslik: "Tarihi Tarsus Sokak Temizliği",
                aciklama: "Eski Tarsus sokakları ve Kleopatra Kapısı çevresini temizleyerek tarihi mirasımıza sahip çıkıyoruz. Belediye ekipleriyle koordineli çalışacağız.",
                tarih: DateTime.UtcNow.AddDays(3),
                konum: "Eski Tarsus Çarşısı, Kleopatra Kapısı",
                kategori: KategoriTip.Cevre,
                kontenjan: 40,
                uyeId: adminId),

            Etkinlik.Olustur(
                baslik: "Tarsus Üniversitesi Kitap Bağış Günü",
                aciklama: "Okuduğunuz ama artık kullanmadığınız kitapları getirin, ihtiyaç sahibi öğrencilere ulaştıralım. Herkese açık.",
                tarih: DateTime.UtcNow.AddDays(20),
                konum: "Tarsus Üniversitesi Ana Kapı Önü",
                kategori: KategoriTip.Diger,
                kontenjan: 100,
                uyeId: adminId)
        };

        // Tüm etkinlikleri admin onaylı yap
        foreach (var etkinlik in etkinlikler)
            etkinlik.AdminOnayla();

        await context.Etkinlikler.AddRangeAsync(etkinlikler);
        await context.SaveChangesAsync();
    }
}
