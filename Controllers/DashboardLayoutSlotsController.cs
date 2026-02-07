using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Entities;
using serveur.Models.Dtos;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardLayoutSlotsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashboardLayoutSlotsController> _logger;

        public DashboardLayoutSlotsController(AppDbContext context, ILogger<DashboardLayoutSlotsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les slots
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DashboardLayoutSlotDto>>> GetAll()
        {
            try
            {
                var slots = await _context.DashboardLayoutSlots
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();

                return Ok(slots.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des slots");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un slot par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DashboardLayoutSlotDto>> GetById(int id)
        {
            try
            {
                var slot = await _context.DashboardLayoutSlots.FindAsync(id);
                if (slot == null)
                {
                    return NotFound();
                }
                return Ok(MapToDto(slot));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du slot {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les slots d'un layout
        /// </summary>
        [HttpGet("layout/{layoutId}")]
        public async Task<ActionResult<IEnumerable<DashboardLayoutSlotDto>>> GetByLayoutId(int layoutId)
        {
            try
            {
                var slots = await _context.DashboardLayoutSlots
                    .Where(s => s.LayoutId == layoutId)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();

                return Ok(slots.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des slots du layout {LayoutId}", layoutId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau slot
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DashboardLayoutSlotDto>> Create([FromBody] DashboardLayoutSlotCreateDto dto)
        {
            try
            {
                var slot = new DashboardLayoutSlot
                {
                    LayoutId = dto.LayoutId,
                    ComponentName = dto.ComponentName,
                    SortOrder = dto.SortOrder,
                    ColXs = dto.ColXs,
                    ColSm = dto.ColSm,
                    ColMd = dto.ColMd,
                    ColLg = dto.ColLg,
                    ColXl = dto.ColXl,
                    OffsetXs = dto.OffsetXs,
                    OffsetMd = dto.OffsetMd,
                    ComponentConfig = dto.ComponentConfig
                };

                _context.DashboardLayoutSlots.Add(slot);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = slot.Id }, MapToDto(slot));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du slot");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un slot
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DashboardLayoutSlotUpdateDto dto)
        {
            try
            {
                var slot = await _context.DashboardLayoutSlots.FindAsync(id);
                if (slot == null)
                {
                    return NotFound();
                }

                slot.ComponentName = dto.ComponentName;
                slot.SortOrder = dto.SortOrder;
                slot.ColXs = dto.ColXs;
                slot.ColSm = dto.ColSm;
                slot.ColMd = dto.ColMd;
                slot.ColLg = dto.ColLg;
                slot.ColXl = dto.ColXl;
                slot.OffsetXs = dto.OffsetXs;
                slot.OffsetMd = dto.OffsetMd;
                slot.ComponentConfig = dto.ComponentConfig;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du slot {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un slot
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var slot = await _context.DashboardLayoutSlots.FindAsync(id);
                if (slot == null)
                {
                    return NotFound();
                }

                _context.DashboardLayoutSlots.Remove(slot);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du slot {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private static DashboardLayoutSlotDto MapToDto(DashboardLayoutSlot slot)
        {
            return new DashboardLayoutSlotDto
            {
                Id = slot.Id,
                LayoutId = slot.LayoutId,
                ComponentName = slot.ComponentName,
                SortOrder = slot.SortOrder,
                ColXs = slot.ColXs,
                ColSm = slot.ColSm,
                ColMd = slot.ColMd,
                ColLg = slot.ColLg,
                ColXl = slot.ColXl,
                OffsetXs = slot.OffsetXs,
                OffsetMd = slot.OffsetMd,
                ComponentConfig = slot.ComponentConfig
            };
        }
    }
}
