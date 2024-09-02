using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace API.Models;
public class RoomDevice
{
    [Key]
    public int RoomDeviceId { get; set; }

    [Required]
    public int RoomId { get; set; }
    public Room Room { get; set; }

    [Required]
    public int DeviceId { get; set; }
    public Device Device { get; set; }

    [Required]
    public bool IsOn { get; set; }  
}
public class CreateRoomDeviceDTO
{
    public int RoomId { get; set; }
    public int DeviceId { get; set; }
    public bool IsOn { get; set; }
}

public class GetRoomDeviceDTO
{
    public int RoomDeviceId { get; set; }
    public int RoomId { get; set; }
    public int DeviceId { get; set; }
    public bool IsOn { get; set; }
}

public class GetDeviceInRoomDTO
{
    public int DeviceId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsOn { get; set; }
}

