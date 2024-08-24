namespace ShorterURL.API.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortendUrl { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ClickCount { get; set; } = 0;
    public byte[]? QrCodeImage { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool isExpired => ExpirationDate.HasValue && DateTime.Now > ExpirationDate.Value;

}
