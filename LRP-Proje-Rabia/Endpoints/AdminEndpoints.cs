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

            // Laboratuvarlar
            group.MapGet("/labs", async (AppDbContext db) => await db.Labs.ToListAsync());
            group.MapPost("/labs", async (Lab lab, AppDbContext db) => {
                db.Labs.Add(lab);
                await db.SaveChangesAsync();
                return Results.Created($"/labs/{lab.Id}", lab);
            });

            // Bilgisayarlar ve Otomatik Kod
            group.MapPost("/computers", async (Computer pc, AppDbContext db) => {
                var pcCount = await db.Computers.CountAsync(c => c.LabId == pc.LabId);
                pc.AssetCode = $"LAB{pc.LabId}-PC-{(pcCount + 1):D2}";
                db.Computers.Add(pc);
                await db.SaveChangesAsync();
                return Results.Ok(pc);
            });

            // Öğrenci Atama (DTO'yu dosyanın en altına ekleyebilirsin)
            group.MapPost("/assign", async (AssignDto req, AppDbContext db) => {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.StudentId)
                           ?? new User { Username = req.StudentId, FullName = req.FullName, Password = "123", Role = "Student" };

                if (user.Id == 0) db.Users.Add(user);
                await db.SaveChangesAsync();

                var pc = await db.Computers.FindAsync(req.ComputerId);
                if (pc != null) { pc.UserId = user.Id; await db.SaveChangesAsync(); }
                return Results.Ok(new { message = "İşlem başarılı" });
            });

            // Öğrenci Bilgisayarını Getir
            group.MapGet("/my-computer/{userId}", async (int userId, AppDbContext db) => {
                var pc = await db.Computers.FirstOrDefaultAsync(c => c.UserId == userId);
                return pc is null ? Results.NotFound() : Results.Ok(pc);
            });
        }
    }

    public record AssignDto(int ComputerId, string StudentId, string FullName);
}
