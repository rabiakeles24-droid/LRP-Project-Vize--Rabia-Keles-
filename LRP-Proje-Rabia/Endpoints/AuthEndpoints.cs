using LRP_Proje_Rabia.Data;
using LRP_Proje_Rabia.Models;
using Microsoft.EntityFrameworkCore;

namespace LRP_Proje_Rabia.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/login", async (User loginUser, AppDbContext db) => {
                var user = await db.Users.FirstOrDefaultAsync(u =>
                    u.Username == loginUser.Username && u.Password == loginUser.Password);

                if (user is null) return Results.Unauthorized();
                return Results.Ok(new { user.Id, user.Username, user.FullName, user.Role });
            });
        }
    }
}
