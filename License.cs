namespace LicenseApiProject.Models
{
    public class License
    {
        public int LicenseID { get; set; }
        public int UserID { get; set; }
        public int DeviceID { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // تفادي تحذير nullable reference types
        public User User { get; set; } = null!;
        public Device Device { get; set; } = null!;
    }
}
