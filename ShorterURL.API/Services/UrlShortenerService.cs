using Microsoft.EntityFrameworkCore;
using ShorterURL.API.Data;

namespace ShorterURL.API.Services;

public class UrlShortenerService(ApplicationDbContext context)
{
    private const int CodeLength = 6;
    private const string Symbols = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int MaxAttempts = 10;

    private readonly ApplicationDbContext _context = context;
    private readonly Random _rand = new Random();

    public async Task<string> GenerateUniqueCodeAsync(string? customCode)
    {
        if (customCode != null && customCode.Length < 4)
        {
            throw new ArgumentException("CustomCode must be at least 4 characters long.");
        }

        int attempts = 0;
        while (attempts < MaxAttempts)
        {
            string code = customCode ?? GenerateRandomCode();
            if (!await _context.Urls.AnyAsync(x => x.Code == code))
            {
                return code;
            }
            attempts++;
        }

        throw new InvalidOperationException("Unable to generate a unique code after multiple attempts.");
    }

    private string GenerateRandomCode()
    {
        var code = new char[CodeLength];
        for (int i = 0; i < CodeLength; i++)
        {
            var index = _rand.Next(Symbols.Length);
            code[i] = Symbols[index];
        }
        return new string(code);
    }
}
