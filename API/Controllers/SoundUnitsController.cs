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
    public class SoundUnitsController : ControllerBase
    {
        private readonly SmartHomeContext _context;

        public SoundUnitsController(SmartHomeContext context)
        {
            _context = context;
        }

        // GET: api/SoundUnits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SoundUnitDTO>>> GetSoundUnits()
        {
            return await _context.SoundUnits
                .Include(su => su.DecibelData)
                .Select(su => new SoundUnitDTO
                {
                    Id = su.Id,
                    UnitName = su.UnitName,
                    DecibelData = su.DecibelData.Select(s => new SoundDTO
                    {
                        Decibel = s.Decibel,
                        Time = s.Time,
                    }).ToList()
                })
                .ToListAsync();
        }

        // GET: api/SoundUnits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SoundUnitDTO>> GetSoundUnit(int id)
        {
            var soundUnit = await _context.SoundUnits
                .Include(su => su.DecibelData)
                .Where(su => su.Id == id)
                .Select(su => new SoundUnitDTO
                {
                    Id = su.Id,
                    UnitName = su.UnitName,
                    DecibelData = su.DecibelData.Select(s => new SoundDTO
                    {
                        Decibel = s.Decibel,
                        Time = s.Time,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (soundUnit == null)
            {
                return NotFound();
            }

            return soundUnit;
        }

        // PUT: api/SoundUnits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSoundUnit(int id, SoundUnitDTO soundUnitDto)
        {
            if (id != soundUnitDto.Id)
            {
                return BadRequest();
            }

            var soundUnit = await _context.SoundUnits.FindAsync(id);
            if (soundUnit == null)
            {
                return NotFound();
            }

            soundUnit.UnitName = soundUnitDto.UnitName;
            // Update other properties as needed

            _context.Entry(soundUnit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SoundUnitExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await WebSocketService.SendMessageToAllAsync($"SoundUnit with id: {id} has been updated");

            return NoContent();
        }

        // POST: api/SoundUnits
        [HttpPost]
        public async Task<ActionResult<SoundUnit>> PostSoundUnit(CreateSoundUnitDTO soundUnitDto)
        {
            var soundUnit = new SoundUnit
            {
                UnitName = soundUnitDto.UnitName,
            };

            _context.SoundUnits.Add(soundUnit);
            await _context.SaveChangesAsync();

            await WebSocketService.SendMessageToAllAsync($"New SoundUnit created with name: {soundUnit.UnitName}");

            return CreatedAtAction("GetSoundUnit", new { id = soundUnit.Id }, soundUnit);
        }

        // DELETE: api/SoundUnits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSoundUnit(int id)
        {
            var soundUnit = await _context.SoundUnits.FindAsync(id);
            if (soundUnit == null)
            {
                return NotFound();
            }

            _context.SoundUnits.Remove(soundUnit);
            await _context.SaveChangesAsync();

            await WebSocketService.SendMessageToAllAsync($"SoundUnit with id: {id} has been deleted");

            return NoContent();
        }

        private bool SoundUnitExists(int id)
        {
            return _context.SoundUnits.Any(e => e.Id == id);
        }
    }
}
