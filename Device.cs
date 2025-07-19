using System.ComponentModel;

namespace LicenseApiProject.Models
{
    public class Device
    {
        public int DeviceID { get; set; }
        public string DeviceIdentifier { get; set; } = string.Empty;

        public ICollection<License> Licenses { get; set; } = new List<License>();
    }
}
