using Microsoft.EntityFrameworkCore;
using ShorterURL.API.Entities;

namespace ShorterURL.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ShortUrl> Urls { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>(builder =>
        {
            builder.Property(x => x.Code).HasMaxLength(20);
            builder.HasIndex(x => x.Code).IsUnique();
        });
    }
}
