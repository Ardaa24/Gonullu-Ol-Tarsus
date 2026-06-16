using GonulluOlTarsus.Domain.Entities;
using GonulluOlTarsus.Domain.Interfaces;
using GonulluOlTarsus.Infrastructure;
using GonulluOlTarsus.Infrastructure.Data;
using GonulluOlTarsus.Services.Abstract;
using GonulluOlTarsus.Services.Concrete;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)));

builder.Services.AddIdentity<Uye, IdentityRole>(options =>
{
    // Şifre politikası
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Hesap kilitleme
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Kullanıcı ayarları
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Geliştirme ortamı
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<TurkceIdentityErrorDescriber>();

// Cookie yapılandırması
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Giris";
    options.LogoutPath = "/Account/Cikis";
    options.AccessDeniedPath = "/Account/ErisimEngellendi";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// İstek Sınırlandırma (Rate Limiting)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("general", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});


builder.Services.AddScoped<IEtkinlikRepository, EfEtkinlikRepository>();
builder.Services.AddScoped<IEtkinlikService, EtkinlikService>();
builder.Services.AddScoped<IYorumRepository, EfYorumRepository>();
builder.Services.AddScoped<IYorumService, YorumService>();
builder.Services.AddHostedService<GonulluOlTarsus.Services.Background.EtkinlikTemizlemeServisi>();

// ──────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

await SeedData.InitializeAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Güvenlik Başlıkları (Security Headers)
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter(); // Routing sonrası, Auth öncesi önerilir.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

