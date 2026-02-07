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
    public class DashboardLayoutsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashboardLayoutsController> _logger;

        public DashboardLayoutsController(AppDbContext context, ILogger<DashboardLayoutsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les layouts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DashboardLayoutDto>>> GetAll()
        {
            try
            {
                var layouts = await _context.DashboardLayouts
                    .Include(l => l.Slots)
                    .OrderBy(l => l.SortOrder)
                    .ToListAsync();

                return Ok(layouts.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des layouts");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un layout par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DashboardLayoutDto>> GetById(int id)
        {
            try
            {
                var layout = await _context.DashboardLayouts
                    .Include(l => l.Slots)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (layout == null)
                {
                    return NotFound();
                }

                return Ok(MapToDto(layout));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du layout {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les layouts par clé de page
        /// </summary>
        [HttpGet("page/{pageKey}")]
        public async Task<ActionResult<IEnumerable<DashboardLayoutDto>>> GetByPageKey(string pageKey)
        {
            try
            {
                var layouts = await _context.DashboardLayouts
                    .Include(l => l.Slots)
                    .Where(l => l.PageKey == pageKey && l.IsActive)
                    .OrderBy(l => l.SortOrder)
                    .ToListAsync();

                return Ok(layouts.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des layouts pour la page {PageKey}", pageKey);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les layouts par rôle et clé de page
        /// </summary>
        [HttpGet("role/{roleId}/page/{pageKey}")]
        public async Task<ActionResult<IEnumerable<DashboardLayoutDto>>> GetByRoleAndPage(int roleId, string pageKey)
        {
            try
            {
                var layouts = await _context.DashboardLayouts
                    .Include(l => l.Slots)
                    .Where(l => l.PageKey == pageKey && l.IsActive && (l.RoleId == roleId || l.RoleId == null))
                    .OrderBy(l => l.SortOrder)
                    .ToListAsync();

                return Ok(layouts.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des layouts pour le rôle {RoleId} et la page {PageKey}", roleId, pageKey);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau layout
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DashboardLayoutDto>> Create([FromBody] DashboardLayoutCreateDto dto)
        {
            try
            {
                var layout = new DashboardLayout
                {
                    RoleId = dto.RoleId,
                    PageKey = dto.PageKey,
                    NameFr = dto.NameFr,
                    NameEn = dto.NameEn,
                    SortOrder = dto.SortOrder,
                    IsDefault = dto.IsDefault,
                    IsActive = dto.IsActive
                };

                // Si ce layout est défini comme par défaut, désactiver les autres
                if (layout.IsDefault)
                {
                    await UnsetDefaultLayouts(layout.PageKey, layout.RoleId);
                }

                _context.DashboardLayouts.Add(layout);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = layout.Id }, MapToDto(layout));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du layout");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un layout
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DashboardLayoutUpdateDto dto)
        {
            try
            {
                var layout = await _context.DashboardLayouts.FindAsync(id);
                if (layout == null)
                {
                    return NotFound();
                }

                layout.RoleId = dto.RoleId;
                layout.PageKey = dto.PageKey;
                layout.NameFr = dto.NameFr;
                layout.NameEn = dto.NameEn;
                layout.SortOrder = dto.SortOrder;
                layout.IsDefault = dto.IsDefault;
                layout.IsActive = dto.IsActive;

                // Si ce layout est défini comme par défaut, désactiver les autres
                if (layout.IsDefault)
                {
                    await UnsetDefaultLayouts(layout.PageKey, layout.RoleId, layout.Id);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du layout {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un layout
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var layout = await _context.DashboardLayouts
                    .Include(l => l.Slots)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (layout == null)
                {
                    return NotFound();
                }

                // Supprimer les slots associés
                _context.DashboardLayoutSlots.RemoveRange(layout.Slots);
                _context.DashboardLayouts.Remove(layout);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du layout {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Synchroniser les slots d'un layout
        /// </summary>
        [HttpPut("{id}/slots")]
        public async Task<ActionResult<DashboardLayoutDto>> SyncSlots(int id, [FromBody] SyncLayoutSlotsDto dto)
        {
            try
            {
                var layout = await _context.DashboardLayouts
                    .Include(l => l.Slots)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (layout == null)
                {
                    return NotFound();
                }

                // Supprimer les slots existants
                _context.DashboardLayoutSlots.RemoveRange(layout.Slots);

                // Créer les nouveaux slots
                if (dto.Slots != null && dto.Slots.Any())
                {
                    foreach (var slotDto in dto.Slots)
                    {
                        var slot = new DashboardLayoutSlot
                        {
                            LayoutId = id,
                            ComponentName = slotDto.ComponentName,
                            SortOrder = slotDto.SortOrder,
                            ColXs = slotDto.ColXs,
                            ColSm = slotDto.ColSm,
                            ColMd = slotDto.ColMd,
                            ColLg = slotDto.ColLg,
                            ColXl = slotDto.ColXl,
                            OffsetXs = slotDto.OffsetXs,
                            OffsetMd = slotDto.OffsetMd,
                            ComponentConfig = slotDto.ComponentConfig
                        };
                        _context.DashboardLayoutSlots.Add(slot);
                    }
                }

                await _context.SaveChangesAsync();

                // Recharger avec les nouveaux slots
                layout = await _context.DashboardLayouts
                    .Include(l => l.Slots)
                    .FirstOrDefaultAsync(l => l.Id == id);

                return Ok(MapToDto(layout));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la synchronisation des slots du layout {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task UnsetDefaultLayouts(string pageKey, int? roleId, int? excludeId = null)
        {
            var query = _context.DashboardLayouts
                .Where(l => l.PageKey == pageKey && l.RoleId == roleId && l.IsDefault);

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }

            var defaultLayouts = await query.ToListAsync();
            foreach (var layout in defaultLayouts)
            {
                layout.IsDefault = false;
            }
        }

        private static DashboardLayoutDto MapToDto(DashboardLayout layout)
        {
            return new DashboardLayoutDto
            {
                Id = layout.Id,
                RoleId = layout.RoleId,
                PageKey = layout.PageKey,
                NameFr = layout.NameFr,
                NameEn = layout.NameEn,
                SortOrder = layout.SortOrder,
                IsDefault = layout.IsDefault,
                IsActive = layout.IsActive,
                Slots = layout.Slots?.OrderBy(s => s.SortOrder).Select(s => new DashboardLayoutSlotDto
                {
                    Id = s.Id,
                    LayoutId = s.LayoutId,
                    ComponentName = s.ComponentName,
                    SortOrder = s.SortOrder,
                    ColXs = s.ColXs,
                    ColSm = s.ColSm,
                    ColMd = s.ColMd,
                    ColLg = s.ColLg,
                    ColXl = s.ColXl,
                    OffsetXs = s.OffsetXs,
                    OffsetMd = s.OffsetMd,
                    ComponentConfig = s.ComponentConfig
                }).ToList() ?? new List<DashboardLayoutSlotDto>()
            };
        }
    }
}
