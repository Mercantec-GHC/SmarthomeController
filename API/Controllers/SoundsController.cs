using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Context;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoundsController : ControllerBase
    {
        private readonly SmartHomeContext _context;

        public SoundsController(SmartHomeContext context)
        {
            _context = context;
        }

        // GET: api/Sounds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sound>>> GetSounds()
        {
            return await _context.Sounds.ToListAsync();
        }

        // GET: api/Sounds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sound>> GetSound(int id)
        {
            var sound = await _context.Sounds.FindAsync(id);

            if (sound == null)
            {
                return NotFound();
            }

            return sound;
        }

        // PUT: api/Sounds/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSound(int id, Sound sound)
        {
            if (id != sound.Id)
            {
                return BadRequest();
            }

            _context.Entry(sound).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SoundExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await WebSocketService.SendMessageToAllAsync($"Sound with id: {id} has been updated");

            return NoContent();
        }

        // POST: api/Sounds
        [HttpPost]
        public async Task<ActionResult<Sound>> PostSound(CreateSoundDTO soundDto)
        {
            var soundUnit = await _context.SoundUnits.FindAsync(soundDto.SoundUnitId);
            if (soundUnit == null)
            {
                return BadRequest("Invalid SoundUnitId");
            }

            var sound = new Sound
            {
                Decibel = soundDto.Decibel,
                Time = soundDto.Time,
                SoundUnitId = soundDto.SoundUnitId,
            };

            _context.Sounds.Add(sound);
            await _context.SaveChangesAsync();

            await WebSocketService.SendMessageToAllAsync($"New sound created with decibel: {sound.Decibel} for SoundUnitId: {sound.SoundUnitId}");

            return CreatedAtAction("GetSound", new { id = sound.Id }, sound);
        }


        // DELETE: api/Sounds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSound(int id)
        {
            var sound = await _context.Sounds.FindAsync(id);
            if (sound == null)
            {
                return NotFound();
            }

            _context.Sounds.Remove(sound);
            await _context.SaveChangesAsync();

            await WebSocketService.SendMessageToAllAsync($"Sound with id: {id} has been deleted");

            return NoContent();
        }

        private bool SoundExists(int id)
        {
            return _context.Sounds.Any(e => e.Id == id);
        }
    }
}
