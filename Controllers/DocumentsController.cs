using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Dtos;
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
        /// Obtenir un document par son ID avec ses banques d'items
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentResponseDto>> GetById(int id)
        {
            try
            {
                var document = await _context.Documents
                    .Include(d => d.DocumentItemBanks)
                    .ThenInclude(dib => dib.ItemBank)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (document == null)
                {
                    return NotFound();
                }

                return MapToResponseDto(document);
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
        /// Créer un nouveau document avec ses banques d'items
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DocumentResponseDto>> Create(CreateDocumentDto dto)
        {
            try
            {
                var document = new Document
                {
                    PedagogicalStructureId = dto.PedagogicalStructureId,
                    DocumentTypeId = dto.DocumentTypeId,
                    ExternalReferenceCode = dto.ExternalReferenceCode,
                    Version = dto.Version,
                    IsActive = dto.IsActive,
                    Status = dto.Status,
                    TitleFr = dto.TitleFr,
                    TitleEn = dto.TitleEn,
                    DescriptionFr = dto.DescriptionFr,
                    DescriptionEn = dto.DescriptionEn,
                    WelcomeMessageFr = dto.WelcomeMessageFr,
                    WelcomeMessageEn = dto.WelcomeMessageEn,
                    CopyrightFr = dto.CopyrightFr,
                    CopyrightEn = dto.CopyrightEn,
                    UrlFr = dto.UrlFr,
                    UrlEn = dto.UrlEn,
                    IsDownloadable = dto.IsDownloadable,
                    IsPublic = dto.IsPublic,
                    IsEditable = dto.IsEditable,
                    EditorSettings = dto.EditorSettings,
                    AuthorId = dto.AuthorId,
                    IsTemplate = dto.IsTemplate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                // Ajouter les associations avec les banques d'items
                if (dto.ItemBankIds != null && dto.ItemBankIds.Any())
                {
                    foreach (var itemBankId in dto.ItemBankIds)
                    {
                        _context.DocumentItemBanks.Add(new DocumentItemBank
                        {
                            DocumentId = document.Id,
                            ItemBankId = itemBankId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                // Recharger le document avec ses banques d'items
                var createdDocument = await _context.Documents
                    .Include(d => d.DocumentItemBanks)
                    .ThenInclude(dib => dib.ItemBank)
                    .FirstOrDefaultAsync(d => d.Id == document.Id);

                return CreatedAtAction(nameof(GetById), new { id = document.Id }, MapToResponseDto(createdDocument));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du document");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un document avec ses banques d'items
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DocumentResponseDto>> Update(int id, UpdateDocumentDto dto)
        {
            try
            {
                var document = await _context.Documents
                    .Include(d => d.DocumentItemBanks)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (document == null)
                {
                    return NotFound();
                }

                // Mettre à jour les propriétés du document
                document.PedagogicalStructureId = dto.PedagogicalStructureId;
                document.DocumentTypeId = dto.DocumentTypeId;
                document.ExternalReferenceCode = dto.ExternalReferenceCode;
                document.Version = dto.Version;
                document.IsActive = dto.IsActive;
                document.Status = dto.Status;
                document.TitleFr = dto.TitleFr;
                document.TitleEn = dto.TitleEn;
                document.DescriptionFr = dto.DescriptionFr;
                document.DescriptionEn = dto.DescriptionEn;
                document.WelcomeMessageFr = dto.WelcomeMessageFr;
                document.WelcomeMessageEn = dto.WelcomeMessageEn;
                document.CopyrightFr = dto.CopyrightFr;
                document.CopyrightEn = dto.CopyrightEn;
                document.UrlFr = dto.UrlFr;
                document.UrlEn = dto.UrlEn;
                document.IsDownloadable = dto.IsDownloadable;
                document.IsPublic = dto.IsPublic;
                document.IsEditable = dto.IsEditable;
                document.EditorSettings = dto.EditorSettings;
                document.AuthorId = dto.AuthorId;
                document.IsTemplate = dto.IsTemplate;
                document.UpdatedAt = DateTime.UtcNow;

                // Mettre à jour les associations avec les banques d'items
                if (dto.ItemBankIds != null)
                {
                    // Supprimer les anciennes associations
                    _context.DocumentItemBanks.RemoveRange(document.DocumentItemBanks);

                    // Ajouter les nouvelles associations
                    foreach (var itemBankId in dto.ItemBankIds)
                    {
                        _context.DocumentItemBanks.Add(new DocumentItemBank
                        {
                            DocumentId = document.Id,
                            ItemBankId = itemBankId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // Recharger le document avec ses banques d'items
                var updatedDocument = await _context.Documents
                    .Include(d => d.DocumentItemBanks)
                    .ThenInclude(dib => dib.ItemBank)
                    .FirstOrDefaultAsync(d => d.Id == id);

                return Ok(MapToResponseDto(updatedDocument));
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

        private static DocumentResponseDto MapToResponseDto(Document document)
        {
            return new DocumentResponseDto
            {
                Id = document.Id,
                PedagogicalStructureId = document.PedagogicalStructureId,
                DocumentTypeId = document.DocumentTypeId,
                ExternalReferenceCode = document.ExternalReferenceCode,
                Version = document.Version,
                IsActive = document.IsActive,
                Status = document.Status,
                TitleFr = document.TitleFr,
                TitleEn = document.TitleEn,
                DescriptionFr = document.DescriptionFr,
                DescriptionEn = document.DescriptionEn,
                WelcomeMessageFr = document.WelcomeMessageFr,
                WelcomeMessageEn = document.WelcomeMessageEn,
                CopyrightFr = document.CopyrightFr,
                CopyrightEn = document.CopyrightEn,
                UrlFr = document.UrlFr,
                UrlEn = document.UrlEn,
                IsDownloadable = document.IsDownloadable,
                IsPublic = document.IsPublic,
                IsEditable = document.IsEditable,
                EditorSettings = document.EditorSettings,
                AuthorId = document.AuthorId,
                IsTemplate = document.IsTemplate,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                ItemBanks = document.DocumentItemBanks?.Select(dib => new ItemBankSummaryDto
                {
                    Id = dib.ItemBank.Id,
                    NameFr = dib.ItemBank.NameFr,
                    NameEn = dib.ItemBank.NameEn
                }).ToList() ?? new List<ItemBankSummaryDto>()
            };
        }
    }
}
