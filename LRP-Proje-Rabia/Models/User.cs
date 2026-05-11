namespace LRP_Proje_Rabia.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } // Öğrenci No veya 'admin'
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // Admin, Student
    }
}
