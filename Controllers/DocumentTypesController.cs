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

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentTypesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DocumentTypesController> _logger;

        public DocumentTypesController(AppDbContext context, ILogger<DocumentTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les types de documents
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentType>>> GetAll()
        {
            try
            {
                return await _context.DocumentTypes.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des types de documents");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un type de document par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentType>> GetById(int id)
        {
            try
            {
                var type = await _context.DocumentTypes.FindAsync(id);
                if (type == null)
                {
                    return NotFound();
                }
                return type;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du type de document {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un type de document par son code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<DocumentType>> GetByCode(string code)
        {
            try
            {
                var type = await _context.DocumentTypes
                    .FirstOrDefaultAsync(t => t.Code == code);
                if (type == null)
                {
                    return NotFound();
                }
                return type;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du type par code {Code}", code);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les types de documents d'une structure pédagogique
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<DocumentType>>> GetByStructure(int structureId)
        {
            try
            {
                return await _context.DocumentTypes
                    .Where(t => t.PedagogicalStructureId == structureId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des types de la structure {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau type de document
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DocumentType>> Create(DocumentType type)
        {
            try
            {
                _context.DocumentTypes.Add(type);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du type de document");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un type de document
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DocumentType type)
        {
            if (id != type.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(type).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TypeExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du type de document {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un type de document
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des documents de ce type
                var hasDocuments = await _context.Documents.AnyAsync(d => d.DocumentTypeId == id);
                if (hasDocuments)
                {
                    return BadRequest("Impossible de supprimer un type utilisé par des documents");
                }

                var type = await _context.DocumentTypes.FindAsync(id);
                if (type == null)
                {
                    return NotFound();
                }

                _context.DocumentTypes.Remove(type);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du type de document {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> TypeExists(int id)
        {
            return await _context.DocumentTypes.AnyAsync(e => e.Id == id);
        }
    }
}
