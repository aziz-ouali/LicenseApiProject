using System.ComponentModel.DataAnnotations;

namespace LicenseApiProject.Models
{
    public class Device
    {
        public int DeviceID { get; set; }

        [Required]
        public string DeviceIdentifier { get; set; } = string.Empty;

        public virtual ICollection<License> Licenses { get; set; } = new List<License>();
    }
}
