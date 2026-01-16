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
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(AppDbContext context, ILogger<DocumentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les documents
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetAll()
        {
            try
            {
                return await _context.Documents
                    .OrderByDescending(d => d.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des documents");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un document par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetById(int id)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return NotFound();
                }
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du document {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les documents par type
        /// </summary>
        [HttpGet("type/{typeId}")]
        public async Task<ActionResult<IEnumerable<Document>>> GetByType(int typeId)
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.DocumentTypeId == typeId)
                    .OrderByDescending(d => d.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des documents par type {TypeId}", typeId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les documents d'une structure pédagogique
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<Document>>> GetByStructure(int structureId)
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.PedagogicalStructureId == structureId)
                    .OrderByDescending(d => d.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des documents de la structure {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les documents d'un auteur
        /// </summary>
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<Document>>> GetByAuthor(int authorId)
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.AuthorId == authorId)
                    .OrderByDescending(d => d.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des documents de l'auteur {AuthorId}", authorId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les documents templates
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<IEnumerable<Document>>> GetTemplates()
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.IsTemplate)
                    .OrderBy(d => d.TitleFr)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des templates");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les documents actifs
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Document>>> GetActive()
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.IsActive)
                    .OrderByDescending(d => d.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des documents actifs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les documents publics
        /// </summary>
        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<Document>>> GetPublic()
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.IsPublic && d.IsActive)
                    .OrderByDescending(d => d.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des documents publics");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau document
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Document>> Create(Document document)
        {
            try
            {
                document.CreatedAt = DateTime.UtcNow;
                document.UpdatedAt = DateTime.UtcNow;

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du document");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un document
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Document document)
        {
            if (id != document.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                document.UpdatedAt = DateTime.UtcNow;
                _context.Entry(document).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DocumentExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du document {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un document
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return NotFound();
                }

                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du document {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> DocumentExists(int id)
        {
            return await _context.Documents.AnyAsync(e => e.Id == id);
        }
    }
}
