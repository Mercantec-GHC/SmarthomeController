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
    public class RoomsController : ControllerBase
    {
        private readonly SmartHomeContext _context;

        public RoomsController(SmartHomeContext context)
        {
            _context = context;
        }

        [HttpGet("WithDevices")]
        public async Task<ActionResult<IEnumerable<RoomWithDevicesDTO>>> GetRoomsWithDevices()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomDevices)
                .ThenInclude(rd => rd.Device)
                .Select(r => new RoomWithDevicesDTO
                {
                    RoomId = r.RoomId,
                    RoomName = r.Name,
                    Devices = r.RoomDevices.Select(rd => new GetDeviceInRoomDTO
                    {
                        DeviceId = rd.Device.DeviceId,
                        Name = rd.Device.Name,
                        Type = rd.Device.Type.ToString(),
                        IsOn = rd.IsOn
                    }).ToList()
                })
                .ToListAsync();

            return rooms;
        }

        [HttpPost]
        public async Task<ActionResult<GetRoomDTO>> PostRoom(CreateRoomDTO roomDTO)
        {
            var room = new Room
            {
                Name = roomDTO.Name
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var getRoomDTO = new GetRoomDTO
            {
                RoomId = room.RoomId,
                Name = room.Name
            };

            // Notify WebSocket clients about the new room
            await WebSocketService.SendMessageToAllAsync($"New room added: {getRoomDTO.Name}");

            return CreatedAtAction("GetRoom", new { id = room.RoomId }, getRoomDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, CreateRoomDTO roomDTO)
        {
            if (id == 0 || roomDTO == null)
            {
                return BadRequest();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.Name = roomDTO.Name;

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                // Notify WebSocket clients about the updated room
                await WebSocketService.SendMessageToAllAsync($"Room updated: {room.Name}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            // Notify WebSocket clients about the deleted room
            await WebSocketService.SendMessageToAllAsync($"Room deleted: {room.Name}");

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
}
