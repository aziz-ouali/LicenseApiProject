using System.ComponentModel;

namespace LicenseApiProject.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<License> Licenses { get; set; } = new List<License>();
    }
}
