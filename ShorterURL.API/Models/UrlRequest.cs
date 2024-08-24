using System.ComponentModel.DataAnnotations;

namespace ShorterURL.API.Models;

public class UrlRequest
{
    public string Url { get; set; }
    [StringLength(20, MinimumLength = 4, ErrorMessage = "CustomCode must be between 4 and 20 characters.")]
    public string? CustomCode { get; set; }
    public int? ExpirationDurationInDays { get; set; }
}
