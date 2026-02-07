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
    public class ExamDocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExamDocumentsController> _logger;

        public ExamDocumentsController(AppDbContext context, ILogger<ExamDocumentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les documents d'un examen
        /// </summary>
        [HttpGet("by-exam/{examId}")]
        public async Task<ActionResult<IEnumerable<ExamDocument>>> GetByExam(int examId)
        {
            try
            {
                return await _context.ExamDocuments
                    .Include(ed => ed.Document)
                    .Where(ed => ed.ExamId == examId)
                    .OrderBy(ed => ed.DisplayOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des documents de l'examen {ExamId}", examId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un examDocument par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDocument>> GetById(int id)
        {
            try
            {
                var examDocument = await _context.ExamDocuments
                    .Include(ed => ed.Exam)
                    .Include(ed => ed.Document)
                    .FirstOrDefaultAsync(ed => ed.Id == id);

                if (examDocument == null)
                {
                    return NotFound();
                }
                return examDocument;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'examDocument {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajouter un document à un examen
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExamDocument>> Create(ExamDocument examDocument)
        {
            try
            {
                examDocument.CreatedAt = DateTime.UtcNow;
                examDocument.UpdatedAt = DateTime.UtcNow;

                _context.ExamDocuments.Add(examDocument);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = examDocument.Id }, examDocument);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'examDocument");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un examDocument
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExamDocument examDocument)
        {
            if (id != examDocument.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                examDocument.UpdatedAt = DateTime.UtcNow;
                _context.Entry(examDocument).State = EntityState.Modified;
                _context.Entry(examDocument).Property(x => x.CreatedAt).IsModified = false;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExamDocumentExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'examDocument {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour l'ordre d'affichage des documents d'un examen
        /// </summary>
        [HttpPut("reorder/{examId}")]
        public async Task<IActionResult> Reorder(int examId, [FromBody] List<int> documentIds)
        {
            try
            {
                var examDocuments = await _context.ExamDocuments
                    .Where(ed => ed.ExamId == examId)
                    .ToListAsync();

                for (int i = 0; i < documentIds.Count; i++)
                {
                    var examDoc = examDocuments.FirstOrDefault(ed => ed.DocumentId == documentIds[i]);
                    if (examDoc != null)
                    {
                        examDoc.DisplayOrder = i;
                        examDoc.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réorganisation des documents de l'examen {ExamId}", examId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un examDocument
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var examDocument = await _context.ExamDocuments.FindAsync(id);
                if (examDocument == null)
                {
                    return NotFound();
                }

                _context.ExamDocuments.Remove(examDocument);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'examDocument {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ExamDocumentExists(int id)
        {
            return await _context.ExamDocuments.AnyAsync(ed => ed.Id == id);
        }
    }
}
