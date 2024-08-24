using Microsoft.EntityFrameworkCore;
using ShorterURL.API.Data;
using ShorterURL.API.Endpoints;
using ShorterURL.API.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("UrlShortener"));
builder.Services.AddScoped<UrlShortenerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapShortUrlEndpoints();

app.Run();