using Microsoft.EntityFrameworkCore;
using ShorterURL.API.Data;
using ShorterURL.API.Entities;
using ShorterURL.API.Models;
using ShorterURL.API.Services;

namespace ShorterURL.API.Endpoints;

public static class ShortUrlEndpoints
{
    public static void MapShortUrlEndpoints(this WebApplication app)
    {
        app.MapPost("api/shorten", async (
            UrlRequest request,
            UrlShortenerService urlService,
            ApplicationDbContext applicationContext,
            HttpContext httpContext) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("Url is invalid.");
            }

            DateTime expirationDate = !request.ExpirationDurationInDays.HasValue || request.ExpirationDurationInDays == 0
                ? DateTime.Now.AddDays(10)
                : DateTime.Now.AddDays(request.ExpirationDurationInDays.Value);

            string code;
            try
            {
                code = await urlService.GenerateUniqueCodeAsync(request.CustomCode);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }

            var shorterUrl = new ShortUrl
            {
                Id = Guid.NewGuid(),
                OriginalUrl = request.Url,
                Code = code,
                ShortendUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
                ExpirationDate = expirationDate
            };

            applicationContext.Add(shorterUrl);
            await applicationContext.SaveChangesAsync();

            return Results.Ok(shorterUrl.ShortendUrl);
        });

        app.MapGet("/api/{code}", async (string code, ApplicationDbContext applicationContext) =>
        {
            var shortendUrl = await applicationContext.Urls.FirstOrDefaultAsync(x => x.Code == code);
            if (shortendUrl is null)
            {
                return Results.NotFound("Url not found.");
            }
            if (shortendUrl.isExpired)
            {
                return Results.NotFound("Url has expired.");
            }
            shortendUrl.ClickCount++;
            await applicationContext.SaveChangesAsync();
            return Results.Redirect(shortendUrl.OriginalUrl);
        });

        app.MapGet("/api/{code}/details", async (string code, ApplicationDbContext applicationContext) =>
        {
            var shortendUrl = await applicationContext.Urls.FirstOrDefaultAsync(x => x.Code == code);

            if (shortendUrl is null)
            {
                return Results.NotFound("Url not found.");
            }

            var details = new
            {
                shortendUrl.ShortendUrl,
                shortendUrl.OriginalUrl,
                shortendUrl.ClickCount,
                shortendUrl.ExpirationDate,
                QrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=150x150&data={shortendUrl.ShortendUrl}"
            };

            return Results.Ok(details);
        });

        app.MapDelete("/api/{code}", async (string code, ApplicationDbContext applicationContext) =>
        {
            var shortenedUrl = await applicationContext.Urls.FirstOrDefaultAsync(x => x.Code == code);

            if (shortenedUrl is null)
            {
                return Results.NotFound("Url not found.");
            }

            applicationContext.Urls.Remove(shortenedUrl);
            await applicationContext.SaveChangesAsync();

            return Results.Ok("Url has been deleted.");
        });
    }
}
