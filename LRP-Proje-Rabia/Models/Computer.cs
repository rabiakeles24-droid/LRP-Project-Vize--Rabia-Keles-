namespace LRP_Proje_Rabia.Models
{
    public class Computer
    {
        public int Id { get; set; }
        public string AssetCode { get; set; } // LAB1-PC-01
        public string Brand { get; set; }
        public string Processor { get; set; }
        public int Ram { get; set; }
        public string Specs { get; set; } // HDMI, Veyon vb.
        public int LabId { get; set; }
        public int? UserId { get; set; } // Atanan Öğrenci
        }
}
