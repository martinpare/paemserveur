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
    public class TitlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TitlesController> _logger;

        public TitlesController(AppDbContext context, ILogger<TitlesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les titres
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Title>>> GetAll()
        {
            try
            {
                return await _context.Titles
                    .OrderBy(t => t.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des titres");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un titre par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Title>> GetById(int id)
        {
            try
            {
                var title = await _context.Titles.FindAsync(id);
                if (title == null)
                {
                    return NotFound();
                }
                return title;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du titre {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les titres actifs
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Title>>> GetActive()
        {
            try
            {
                return await _context.Titles
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des titres actifs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les enfants d'un titre
        /// </summary>
        [HttpGet("{id}/children")]
        public async Task<ActionResult<IEnumerable<Title>>> GetChildren(int id)
        {
            try
            {
                return await _context.Titles
                    .Where(t => t.ParentId == id)
                    .OrderBy(t => t.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des enfants du titre {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les titres en structure arborescente
        /// </summary>
        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<TitleTreeDto>>> GetTree()
        {
            try
            {
                var allTitles = await _context.Titles
                    .OrderBy(t => t.Order)
                    .ToListAsync();

                var roots = allTitles.Where(t => t.ParentId == null);
                return Ok(BuildTitleTree(roots, allTitles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'arbre des titres");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private List<TitleTreeDto> BuildTitleTree(IEnumerable<Title> nodes, List<Title> allTitles)
        {
            return nodes.Select(t => new TitleTreeDto
            {
                Id = t.Id,
                Code = t.Code,
                MaleLabelFr = t.MaleLabelFr,
                FemaleLabelFr = t.FemaleLabelFr,
                MaleLabelEn = t.MaleLabelEn,
                FemaleLabelEn = t.FemaleLabelEn,
                Order = t.Order,
                IsActive = t.IsActive,
                Children = BuildTitleTree(allTitles.Where(c => c.ParentId == t.Id), allTitles)
            }).ToList();
        }

        /// <summary>
        /// Créer un nouveau titre
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Title>> Create(Title title)
        {
            try
            {
                _context.Titles.Add(title);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = title.Id }, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du titre");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un titre
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Title title)
        {
            if (id != title.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(title).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TitleExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du titre {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un titre
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des enfants
                var hasChildren = await _context.Titles.AnyAsync(t => t.ParentId == id);
                if (hasChildren)
                {
                    return BadRequest("Impossible de supprimer un titre ayant des enfants");
                }

                var title = await _context.Titles.FindAsync(id);
                if (title == null)
                {
                    return NotFound();
                }

                _context.Titles.Remove(title);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du titre {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> TitleExists(int id)
        {
            return await _context.Titles.AnyAsync(e => e.Id == id);
        }
    }
}
