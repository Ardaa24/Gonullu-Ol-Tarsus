using System.ComponentModel.DataAnnotations;
using GonulluOlTarsus.Domain.Enums;

namespace GonulluOlTarsus.Models;

/// <summary>
/// Etkinlik oluşturma formu için ViewModel.
/// Data Annotation'lar ile validation kuralları tanımlanmıştır.
/// </summary>
public class EtkinlikOlusturViewModel
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Başlık 5-200 karakter arasında olmalıdır.")]
    [Display(Name = "Etkinlik Başlığı")]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(4000, MinimumLength = 20, ErrorMessage = "Açıklama 20-4000 karakter arasında olmalıdır.")]
    [Display(Name = "Açıklama")]
    public string Aciklama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tarih zorunludur.")]
    [Display(Name = "Etkinlik Tarihi")]
    public DateTime Tarih { get; set; } = DateTime.Today.AddDays(7);

    [Required(ErrorMessage = "Konum zorunludur.")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "Konum 3-300 karakter arasında olmalıdır.")]
    [Display(Name = "Konum / Adres")]
    public string Konum { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori seçiniz.")]
    [Display(Name = "Etkinlik Kategorisi")]
    public KategoriTip Kategori { get; set; }

    [Required(ErrorMessage = "Kontenjan zorunludur.")]
    [Range(1, 1000, ErrorMessage = "Kontenjan 1-1000 arasında olmalıdır.")]
    [Display(Name = "Kontenjan (Kişi Sayısı)")]
    public int Kontenjan { get; set; } = 20;
}
