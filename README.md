# Gönüllü Ol | Tarsus 🌍🤝

Gönüllü Ol | Tarsus, Tarsus şehrindeki yerel gönüllülük faaliyetlerini, sosyal sorumluluk projelerini ve topluluk etkinliklerini tek bir çatı altında toplamayı amaçlayan, açık kaynak kodlu bir dijital platformdur. 

Bu proje; modern yazılım mimarileri (Rich Domain Model, DDD Esasları, Repository Pattern) kullanılarak sürdürülebilir, güvenli ve genişletilebilir bir yapıda geliştirilmiştir.

## 🚀 Öne Çıkan Özellikler

- **Zengin Alan Modeli (Rich Domain Model):** Tüm iş kuralları ve validasyonlar (kontenjan takibi, mükerrer kayıt engelleme, geçmiş tarihli etkinlik kontrolü) doğrudan Domain varlıkları (`Etkinlik`, `Uye`) içinde kapsüllenmiştir.
- **Gelişmiş Güvenlik Katmanı:** ASP.NET Core Identity altyapısı, özelleştirilmiş Türkçe hata mesajları, brute-force korumalı hesap kilitleme sistemi ve sıkılaştırılmış Cookie politikaları.
- **İstek Sınırlandırma (Rate Limiting):** API ve sayfa isteklerine karşı uygulamayı DDOS ve kötüye kullanımdan koruyan dahili Rate Limiter mekanizması.
- **Güvenlik Başlıkları (Security Headers):** `X-Frame-Options (DENY)`, `X-Content-Type-Options (nosniff)` gibi modern tarayıcı güvenlik başlıkları.
- **Arka Plan Görevleri (Background Services):** Süresi geçmiş veya geçerliliğini yitirmiş etkinlikleri otomatik yöneten entegre `HostedService` altyapısı.

## 🛠️ Kullanılan Teknolojiler ve Bağımlılıklar

- **Backend / Core:** .NET 10.0 (ASP.NET Core MVC)
- **Database / ORM:** Microsoft SQL Server & Entity Framework Core 9.0+
- **Identity:** ASP.NET Core Identity Entity Framework Integration
- **Frontend / UI:** HTML5, Tailwind CSS, JavaScript (Razor Views entegrasyonu ile)

## 🏗️ Proje Mimarisi

Proje, temiz kod (Clean Code) ve esneklik ilkelerine sadık kalınarak katmanlı bir yapıda tasarlanmıştır:

- **Domain:** İş mantığının, enum yapılarının ve ana entity modellerinin bulunduğu, hiçbir dış kütüphaneye bağımlı olmayan çekirdek katman.
- **Infrastructure:** Veritabanı bağlamı (`AppDbContext`), EF veri depoları (`EfRepository`) ve veritabanı göç süreçlerinin (`Migrations`) yer aldığı katman.
- **Services:** İş mantığı servis arayüzleri ve somut sınıfları ile arka plan servislerinin yönetildiği katman.
- **Web / Presentation (Controllers & Views):** Kullanıcı etkileşimini sağlayan, Tailwind CSS ile modernleştirilmiş arayüze sahip MVC mimarisinin kurulduğu katman.

## 💻 Kurulum ve Çalıştırma

### Gereksinimler
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- MS SQL Server (LocalDB veya Express sürümü yeterlidir)
- Tercihen Visual Studio 2022 veya VS Code

### Adımlar

1. **Projeyi Klonlayın:**
   git clone https://github.com/ardaa24/gonullu-ol-tarsus.git
   cd gonullu-ol-tarsus

2. **Veritabanı Bağlantısını Düzenleyin:**
   `appsettings.json` dosyasını açarak `DefaultConnection` bağlantı dizesini kendi SQL Server yapınıza göre güncelleyin:
   "ConnectionStrings": { "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GonulluOlTarsusDb;Trusted_Connection=True;MultipleActiveResultSets=true" }

3. **Bağımlılıkları Yükleyin ve Veritabanını Güncelleyin:**
   Terminal üzerinden paketleri geri yükleyin ve migrasyonları uygulayarak veritabanının otomatik oluşmasını sağlayın:
   dotnet restore
   dotnet ef database update --project ../Infrastructure/

4. **Projeyi Çalıştırın:**
   dotnet run
   Uygulama ayağa kalktığında terminalde görünen URL'yi tarayıcınızda açabilirsiniz. Proje ilk açılışta `SeedData` mekanizması sayesinde örnek verilerle birlikte başlayacaktır.

## 🤝 Katkıda Bulunma (Contributing)

Bu proje açık kaynaklı bir topluluk projesidir. Tarsus'taki dijital dönüşüme ve gönüllülük ağına katkı sağlamak isteyen herkesin desteklerini bekliyoruz! 

Nasıl katkıda bulunabilirsiniz?
1. Bu depoyu Fork edin.
2. Yeni bir özellik veya hata düzeltmesi için bir branch açın: git checkout -b feature/yeniOzellik
3. Değişikliklerinizi commit edin: git commit -am 'Yeni bir özellik eklendi'
4. Branch'inizi push edin: git push origin feature/yeniOzellik
5. Bir Pull Request (PR) oluşturun.

*Katkı sağlarken kodun mevcut mimari kurallarına (Domain kurallarının korunması, kapsülleme, temiz kod prensipleri) sadık kalmaya özen gösteriniz.*

## 📄 Lisans

Bu proje MIT Lisansı ile lisanslanmıştır. Bu; kodları dilediğiniz gibi kişisel projelerinizde kullanabileceğiniz, değiştirebileceğiniz ve paylaşabileceğiniz anlamına gelir. Detaylar için LICENSE dosyasına göz atabilirsiniz.

---
⭐ Projeyi beğendiyseniz yıldız (star) vererek destek olmayı unutmayın!
