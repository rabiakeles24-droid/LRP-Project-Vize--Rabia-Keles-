using LRP_Proje_Rabia.Data;
using LRP_Proje_Rabia.Models;
using Microsoft.EntityFrameworkCore;

namespace LRP_Proje_Rabia.Endpoints
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api");

            // --- LABORATUVAR İŞLEMLERİ ---
            group.MapGet("/labs", async (AppDbContext db) =>
                await db.Labs.ToListAsync());

            group.MapPost("/labs", async (Lab lab, AppDbContext db) =>
            {
                db.Labs.Add(lab);
                await db.SaveChangesAsync();
                return Results.Created($"/labs/{lab.Id}", lab);
            });

            group.MapDelete("/labs/{id}", async (int id, AppDbContext db) =>
            {
                var lab = await db.Labs.FindAsync(id);
                if (lab == null) return Results.NotFound();
                db.Labs.Remove(lab);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // --- BİLGİSAYAR İŞLEMLERİ ---
            group.MapGet("/computers", async (AppDbContext db) =>
                await db.Computers.ToListAsync());

            group.MapPost("/computers", async (Computer pc, AppDbContext db) =>
            {
                var pcCount = await db.Computers.CountAsync(c => c.LabId == pc.LabId);
                // AssetCode (Büyük harf uyumuna dikkat edildi)
                pc.AssetCode = $"LAB{pc.LabId}-PC-{(pcCount + 1):D2}";
                db.Computers.Add(pc);
                await db.SaveChangesAsync();
                return Results.Ok(pc);
            });

            group.MapDelete("/computers/{id}", async (int id, AppDbContext db) =>
            {
                var pc = await db.Computers.FindAsync(id);
                if (pc == null) return Results.NotFound();
                db.Computers.Remove(pc);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // --- ZİMMETLEME VE ÖĞRENCİ İŞLEMLERİ ---
            group.MapPost("/assign", async (AssignDto req, AppDbContext db) =>
            {
                // Öğrenciyi bul veya yeni oluştur
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.StudentId)
                    ?? new User
                    {
                        Username = req.StudentId,
                        FullName = req.FullName,
                        Password = "123",
                        Role = "Student"
                    };

                if (user.Id == 0) db.Users.Add(user);
                await db.SaveChangesAsync();

                // Bilgisayara ata
                var pc = await db.Computers.FindAsync(req.ComputerId);
                if (pc != null)
                {
                    pc.UserId = user.Id;
                    await db.SaveChangesAsync();
                }
                return Results.Ok(new { message = "İşlem başarılı" });
            });

            // Aktif zimmetleri listele (Sağdaki tablo için)
            group.MapGet("/assignments", async (AppDbContext db) =>
            {
                var list = await db.Computers
                    .Where(c => c.UserId != null)
                    .Join(db.Users, pc => pc.UserId, user => user.Id, (pc, user) => new
                    {
                        pcId = pc.Id,
                        assetCode = pc.AssetCode, // Büyük harf düzeltildi
                        studentName = user.FullName,
                        studentId = user.Username
                    }).ToListAsync();
                return Results.Ok(list);
            });

            // Zimmetli öğrenci adını güncelleme
            group.MapPut("/assign/update/{pcId}", async (int pcId, UpdateUserDto req, AppDbContext db) =>
            {
                var pc = await db.Computers.FirstOrDefaultAsync(c => c.Id == pcId);
                if (pc != null && pc.UserId != null)
                {
                    var user = await db.Users.FindAsync(pc.UserId);
                    if (user != null)
                    {
                        user.FullName = req.FullName;
                        await db.SaveChangesAsync();
                    }
                }
                return Results.Ok();
            });

            // Zimmet kaldır (Silme)
            group.MapDelete("/assign/{pcId}", async (int pcId, AppDbContext db) =>
            {
                var pc = await db.Computers.FindAsync(pcId);
                if (pc != null)
                {
                    pc.UserId = null;
                    await db.SaveChangesAsync();
                }
                return Results.Ok();
            });

            // Öğrenci kendi bilgisayarını görsün
            group.MapGet("/my-computer/{userId}", async (int userId, AppDbContext db) =>
            {
                var pc = await db.Computers.FirstOrDefaultAsync(c => c.UserId == userId);
                return pc is null ? Results.NotFound() : Results.Ok(pc);
            });

            group.MapGet("/student/pc/{userId}", async (int userId, AppDbContext db) => {
                var pc = await db.Computers.FirstOrDefaultAsync(c => c.UserId == userId);
                return pc is null ? Results.NotFound() : Results.Ok(pc);
            });
        }
    }

    // Kırmızı çizgi hatalarını önleyen tanımlar
    public record UpdateUserDto(string FullName);
    public record AssignDto(int ComputerId, string StudentId, string FullName);
}