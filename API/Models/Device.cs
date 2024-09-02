using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DeviceType Type { get; set; }

        // Navigation property for the many-to-many relationship with Room
        public ICollection<RoomDevice> RoomDevices { get; set; }
    }
    public class CreateDeviceDTO
    {
        public string Name { get; set; }
        public DeviceType Type { get; set; }
    }

    public class GetDeviceDTO
    {
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public enum DeviceType
    {
        Lys,
        Tv,
        Radio
    }

}

