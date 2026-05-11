using Microsoft.EntityFrameworkCore;
using LRP_Proje_Rabia.Models;

namespace LRP_Proje_Rabia.Data

{
    public class AppDbContext : DbContext
    {
        // Constructor (Yapıcı Metot): Ayarların dışarıdan (Program.cs'den) gelmesini sağlar.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet: Veritabanındaki "Computers" tablosunu temsil eder. 
        // Bu satır sayesinde "C# listesiyle oynar gibi" veritabanıyla işlem yapacağız.
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<User> Users { get; set; }


    }
}
