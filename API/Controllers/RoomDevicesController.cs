using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Context;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomDevicesController : ControllerBase
    {
        private readonly SmartHomeContext _context;

        public RoomDevicesController(SmartHomeContext context)
        {
            _context = context;
        }

       

        // GET: api/RoomDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetRoomDeviceDTO>>> GetRoomDevices()
        {
            return await _context.RoomDevices
                .Select(rd => new GetRoomDeviceDTO
                {
                    RoomDeviceId = rd.RoomDeviceId,
                    RoomId = rd.RoomId,
                    DeviceId = rd.DeviceId,
                    IsOn = rd.IsOn
                })
                .ToListAsync();
        }

        // GET: api/RoomDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetRoomDeviceDTO>> GetRoomDevice(int id)
        {
            var roomDevice = await _context.RoomDevices
                .Select(rd => new GetRoomDeviceDTO
                {
                    RoomDeviceId = rd.RoomDeviceId,
                    RoomId = rd.RoomId,
                    DeviceId = rd.DeviceId,
                    IsOn = rd.IsOn
                })
                .FirstOrDefaultAsync(rd => rd.RoomDeviceId == id);

            if (roomDevice == null)
            {
                return NotFound();
            }

            return roomDevice;
        }

        // PUT: api/RoomDevices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomDevice(int id, CreateRoomDeviceDTO roomDeviceDTO)
        {
            if (id == 0 || roomDeviceDTO == null)
            {
                return BadRequest();
            }

            var roomDevice = await _context.RoomDevices.FindAsync(id);
            if (roomDevice == null)
            {
                return NotFound();
            }

            // Update the entity with the data from the DTO
            roomDevice.RoomId = roomDeviceDTO.RoomId;
            roomDevice.DeviceId = roomDeviceDTO.DeviceId;
            roomDevice.IsOn = roomDeviceDTO.IsOn;

            _context.Entry(roomDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (roomDevice.IsOn)
            {
                await WebSocketService.SendMessageToAllAsync($"Device with ID: {roomDeviceDTO.DeviceId} turned on");
            } else
            {
                await WebSocketService.SendMessageToAllAsync($"Device with ID: {roomDeviceDTO.DeviceId} turned off");
            }

            return NoContent();
        }

        // POST: api/RoomDevices
        [HttpPost]
        public async Task<ActionResult<GetRoomDeviceDTO>> PostRoomDevice(CreateRoomDeviceDTO roomDeviceDTO)
        {
            var roomDevice = new RoomDevice
            {
                RoomId = roomDeviceDTO.RoomId,
                DeviceId = roomDeviceDTO.DeviceId,
                IsOn = roomDeviceDTO.IsOn
            };

            _context.RoomDevices.Add(roomDevice);
            await _context.SaveChangesAsync();

            var getRoomDeviceDTO = new GetRoomDeviceDTO
            {
                RoomDeviceId = roomDevice.RoomDeviceId,
                RoomId = roomDevice.RoomId,
                DeviceId = roomDevice.DeviceId,
                IsOn = roomDevice.IsOn
            };
            await WebSocketService.SendMessageToAllAsync($"Device with ID: {roomDeviceDTO.DeviceId} added to Room: {roomDevice.Room}");
            return CreatedAtAction("GetRoomDevice", new { id = roomDevice.RoomDeviceId }, getRoomDeviceDTO);
        }

        // DELETE: api/RoomDevices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomDevice(int id)
        {
            var roomDevice = await _context.RoomDevices.FindAsync(id);
            if (roomDevice == null)
            {
                return NotFound();
            }

            _context.RoomDevices.Remove(roomDevice);
            await _context.SaveChangesAsync();
            await WebSocketService.SendMessageToAllAsync($"Room deleted: {id}");

            return NoContent();
        }

        private bool RoomDeviceExists(int id)
        {
            return _context.RoomDevices.Any(e => e.RoomDeviceId == id);
        }
    }
}
