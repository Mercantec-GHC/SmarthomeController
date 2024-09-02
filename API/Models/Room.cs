using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace API.Models;

public class Room
{
    [Key]
    public int RoomId { get; set; }

    [Required]
    public string Name { get; set; }

    // Navigation property for the many-to-many relationship with Device
    public ICollection<RoomDevice> RoomDevices { get; set; }
}

public class CreateRoomDTO
{
    public string Name { get; set; }
}

public class GetRoomDTO
{
    public int RoomId { get; set; }
    public string Name { get; set; }
}

public class RoomWithDevicesDTO
{
    public string RoomName { get; set; }
    public int RoomId { get; set; }
    public List<GetDeviceInRoomDTO> Devices { get; set; }
}