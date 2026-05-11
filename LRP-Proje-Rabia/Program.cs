using LRP_Proje_Rabia.Models;
using LRP_Proje_Rabia.Data;
using LRP_Proje_Rabia.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Veritabaný servisini "Mutfak" kýsmýna ekliyoruz.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Configuration...: Az önce appsettings.json'a yazdýðýmýz yolu oku demektir.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapAdminEndpoints();
app.MapAuthEndpoints();


app.Run();

