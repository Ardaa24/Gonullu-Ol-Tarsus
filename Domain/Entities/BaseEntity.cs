namespace GonulluOlTarsus.Domain.Entities;

/// <summary>
/// Tüm domain entity'lerinin miras aldığı temel sınıf.
/// Id, audit alanları ve soft-delete desteği sağlar.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime OlusturulmaTarihi { get; protected set; } = DateTime.UtcNow;
    public DateTime? GuncellenmeTarihi { get; protected set; }
    public bool AktifMi { get; protected set; } = true;

    protected void GuncellenmeTarihiniAyarla() =>
        GuncellenmeTarihi = DateTime.UtcNow;

    public void Pasifles() => AktifMi = false;
    public void Aktifles() => AktifMi = true;
}
