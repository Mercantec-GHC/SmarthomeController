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
    public class DevicesController : ControllerBase
    {
        private readonly SmartHomeContext _context;

        public DevicesController(SmartHomeContext context)
        {
            _context = context;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDeviceDTO>>> GetDevices()
        {
            return await _context.Devices
                .Select(d => new GetDeviceDTO
                {
                    DeviceId = d.DeviceId,
                    Name = d.Name,
                    Type = d.Type.ToString()
                })
                .ToListAsync();
        }

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetDeviceDTO>> GetDevice(int id)
        {
            var device = await _context.Devices
                .Select(d => new GetDeviceDTO
                {
                    DeviceId = d.DeviceId,
                    Name = d.Name,
                    Type = d.Type.ToString() // Convert enum to string
                })
                .FirstOrDefaultAsync(d => d.DeviceId == id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // PUT: api/Devices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(int id, CreateDeviceDTO deviceDTO)
        {
            if (id == 0 || deviceDTO == null)
            {
                return BadRequest();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            // Update the entity with the data from the DTO
            device.Name = deviceDTO.Name;
            device.Type = deviceDTO.Type;

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await WebSocketService.SendMessageToAllAsync($"{deviceDTO.Name} have been changed");

            return NoContent();
        }

        // POST: api/Devices
        [HttpPost]
        public async Task<ActionResult<GetDeviceDTO>> PostDevice(CreateDeviceDTO deviceDTO)
        {
            var device = new Device
            {
                Name = deviceDTO.Name,
                Type = deviceDTO.Type
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            var getDeviceDTO = new GetDeviceDTO
            {
                DeviceId = device.DeviceId,
                Name = device.Name,
                Type = device.Type.ToString() // Convert enum to string
            };
            await WebSocketService.SendMessageToAllAsync($"New device created with name: {deviceDTO.Name}");

            return CreatedAtAction("GetDevice", new { id = device.DeviceId }, getDeviceDTO);
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            await WebSocketService.SendMessageToAllAsync($"Device deleted");

            return NoContent();
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.DeviceId == id);
        }
    }
}
